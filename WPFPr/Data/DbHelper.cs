using Microsoft.Data.SqlClient;
using BuilderPC.Models;

namespace BuilderPC.Data;

public static class DbHelper
{
    private static SqlConnection OpenConnection()
    {
        var conn = new SqlConnection(App.ConnectionString);
        conn.Open();
        return conn;
    }

    // ─── Справочники ────────────────────────────────────────────────────────────

    public static List<PartType> GetPartTypes()
    {
        var list = new List<PartType>();
        using var conn = OpenConnection();
        using var cmd = new SqlCommand("SELECT id, name FROM parttype$ ORDER BY name", conn);
        using var rdr = cmd.ExecuteReader();
        while (rdr.Read())
            list.Add(new PartType { Id = rdr.GetInt32(0), Name = rdr.GetString(1) });
        return list;
    }

    public static List<Manufacturer> GetManufacturers(int? partTypeId = null)
    {
        var list = new List<Manufacturer>();
        list.Add(new Manufacturer { Id = 0, Name = "Все производители" });

        var sql = partTypeId.HasValue
            ? @"SELECT DISTINCT m.id, m.name FROM manufacturer$ m
                JOIN basepart$ bp ON bp.manufacturerid = m.id
                WHERE bp.parttypeid = @typeId ORDER BY m.name"
            : "SELECT id, name FROM manufacturer$ ORDER BY name";

        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        if (partTypeId.HasValue)
            cmd.Parameters.AddWithValue("@typeId", partTypeId.Value);
        using var rdr = cmd.ExecuteReader();
        while (rdr.Read())
            list.Add(new Manufacturer { Id = rdr.GetInt32(0), Name = rdr.GetString(1) });
        return list;
    }

    // ─── Загрузка комплектующих ─────────────────────────────────────────────────

    public static List<Part> GetParts(int partTypeId, string? search, int? manufacturerId)
    {
        // Получаем название типа для формирования запроса со спецификациями
        string typeName = GetTypeName(partTypeId);
        string specJoin = BuildSpecJoin(typeName);
        string specSelect = BuildSpecSelect(typeName);

        var sql = $@"
            SELECT bp.id, bp.name, m.name, bp.manufacturerid, bp.parttypeid, 
                   bp.image, CAST(bp.price AS DECIMAL(18,2)), pt.name {specSelect}
            FROM basepart$ bp
            JOIN manufacturer$ m ON bp.manufacturerid = m.id
            JOIN parttype$ pt ON bp.parttypeid = pt.id
            {specJoin}
            WHERE bp.parttypeid = @typeId
            {(manufacturerId.HasValue && manufacturerId > 0 ? "AND bp.manufacturerid = @mfrId" : "")}
            {(!string.IsNullOrWhiteSpace(search) ? "AND bp.name LIKE @search" : "")}
            ORDER BY bp.name";

        var list = new List<Part>();
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@typeId", partTypeId);
        if (manufacturerId.HasValue && manufacturerId > 0)
            cmd.Parameters.AddWithValue("@mfrId", manufacturerId.Value);
        if (!string.IsNullOrWhiteSpace(search))
            cmd.Parameters.AddWithValue("@search", $"%{search}%");

        using var rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            var part = new Part
            {
                Id = rdr.GetInt32(0),
                Name = rdr.GetString(1),
                Manufacturer = rdr.GetString(2),
                ManufacturerId = rdr.GetInt32(3),
                PartTypeId = rdr.GetInt32(4),
                Image = rdr.IsDBNull(5) ? "" : rdr.GetString(5),
                Price = rdr.GetDecimal(6),
                PartTypeName = rdr.GetString(7),
                Specs = ParseSpecs(typeName, rdr)
            };
            list.Add(part);
        }
        return list;
    }

    private static string GetTypeName(int partTypeId)
    {
        using var conn = OpenConnection();
        using var cmd = new SqlCommand("SELECT name FROM parttype$ WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", partTypeId);
        return cmd.ExecuteScalar()?.ToString()?.ToLower() ?? "";
    }

    private static string BuildSpecJoin(string typeName) => typeName switch
    {
        var t when t.Contains("процессор") || t.Contains("cpu") =>
            @"LEFT JOIN cpu$ c ON c.id = bp.id
              LEFT JOIN socket$ sk ON sk.id = c.socketid
              LEFT JOIN igpu$ ig ON ig.id = c.igpuid",
        var t when t.Contains("видеокарт") || t.Contains("gpu") =>
            @"LEFT JOIN gpu$ g ON g.id = bp.id
              LEFT JOIN gpuinterface$ gi ON gi.id = g.gpuinterfaceid",
        var t when t.Contains("оперативн") || t.Contains("ram") || t.Contains("память") =>
            @"LEFT JOIN ram$ r ON r.id = bp.id
              LEFT JOIN memorytype$ mt ON mt.id = r.memorytypeid",
        var t when t.Contains("материнск") || t.Contains("motherboard") =>
            @"LEFT JOIN motherboard$ mb ON mb.id = bp.id
              LEFT JOIN socket$ sk ON sk.id = mb.socketid
              LEFT JOIN formfactor$ ff ON ff.id = mb.formfactorid
              LEFT JOIN memorytype$ mt ON mt.id = mb.memorytypeid",
        var t when t.Contains("блок питания") || t.Contains("psu") || t.Contains("power") =>
            @"LEFT JOIN powersupply$ ps ON ps.id = bp.id
              LEFT JOIN certificate$ cert ON cert.id = ps.certificationid
              LEFT JOIN fandimension$ fd ON fd.id = ps.fandimensionid",
        var t when t.Contains("кулер") || t.Contains("cooler") =>
            @"LEFT JOIN processorcooler$ pc ON pc.id = bp.id
              LEFT JOIN fandimension$ fd ON fd.id = pc.fandimensionid",
        var t when t.Contains("корпус") || t.Contains("case") =>
            @"LEFT JOIN case$ cs ON cs.id = bp.id
              LEFT JOIN casesize$ csz ON csz.id = cs.sizeid",
        var t when t.Contains("жёсткий") || t.Contains("hdd") =>
            @"LEFT JOIN storagedevice$ sd ON sd.id = bp.id
              LEFT JOIN storagedeviceinterface$ sdi ON sdi.id = sd.storagedeviceinterfaceid
              LEFT JOIN hdd$ hd ON hd.id = bp.id",
        var t when t.Contains("ssd") || t.Contains("накопитель") =>
            @"LEFT JOIN storagedevice$ sd ON sd.id = bp.id
              LEFT JOIN storagedeviceinterface$ sdi ON sdi.id = sd.storagedeviceinterfaceid
              LEFT JOIN ssd$ ss ON ss.id = bp.id",
        _ => ""
    };

    private static string BuildSpecSelect(string typeName) => typeName switch
    {
        var t when t.Contains("процессор") || t.Contains("cpu") =>
            ", sk.name, c.numberofcores, c.basecorefrequency, c.maxcorefrequency, c.thermalpower, c.cachel3, ig.name",
        var t when t.Contains("видеокарт") || t.Contains("gpu") =>
            ", gi.name, g.chipfrequency, g.videomemory, g.memorybus, g.recommendpower",
        var t when t.Contains("оперативн") || t.Contains("ram") || t.Contains("память") =>
            ", mt.name, r.capacity, r.count, r.ghz, r.timings",
        var t when t.Contains("материнск") || t.Contains("motherboard") =>
            ", sk.name, ff.name, mt.name, mb.memoryslots, mb.pcislots, mb.sataports",
        var t when t.Contains("блок питания") || t.Contains("psu") || t.Contains("power") =>
            ", ps.power, cert.name, fd.name",
        var t when t.Contains("кулер") || t.Contains("cooler") =>
            ", fd.name, pc.heatpipes, pc.minspeed, pc.maxspeed, pc.noiselevel",
        var t when t.Contains("корпус") || t.Contains("case") =>
            ", csz.name, cs.expansionslots, cs.fans",
        var t when t.Contains("жёсткий") || t.Contains("hdd") =>
            ", sd.capacity, sdi.name, hd.rotationspeed",
        var t when t.Contains("ssd") || t.Contains("накопитель") =>
            ", sd.capacity, sdi.name, ss.tbw",
        _ => ""
    };

    private static Dictionary<string, string> ParseSpecs(string typeName, SqlDataReader rdr)
    {
        var specs = new Dictionary<string, string>();
        try
        {
            int col = 8; // 0..7 — базовые поля
            if (typeName.Contains("процессор") || typeName.Contains("cpu"))
            {
                specs["Сокет"] = rdr.IsDBNull(col) ? "—" : rdr.GetString(col++); else col++;
                specs["Ядра"] = rdr.IsDBNull(col) ? "—" : rdr.GetInt32(col).ToString(); col++;
                specs["Базовая частота"] = rdr.IsDBNull(col) ? "—" : $"{rdr.GetDouble(col):F1} ГГц"; col++;
                specs["Макс. частота"] = rdr.IsDBNull(col) ? "—" : $"{rdr.GetDouble(col):F1} ГГц"; col++;
                specs["TDP"] = rdr.IsDBNull(col) ? "—" : $"{rdr.GetInt32(col)} Вт"; col++;
                specs["Кэш L3"] = rdr.IsDBNull(col) ? "—" : $"{rdr.GetInt32(col)} МБ"; col++;
                if (!rdr.IsDBNull(col)) specs["iGPU"] = rdr.GetString(col); col++;
            }
            else if (typeName.Contains("видеокарт") || typeName.Contains("gpu"))
            {
                specs["Интерфейс"] = rdr.IsDBNull(col) ? "—" : rdr.GetString(col); col++;
                specs["Частота чипа"] = rdr.IsDBNull(col) ? "—" : $"{rdr.GetInt32(col)} МГц"; col++;
                specs["Память"] = rdr.IsDBNull(col) ? "—" : $"{rdr.GetInt32(col)} ГБ"; col++;
                specs["Шина"] = rdr.IsDBNull(col) ? "—" : $"{rdr.GetInt32(col)} бит"; col++;
                specs["Реком. БП"] = rdr.IsDBNull(col) ? "—" : $"{rdr.GetInt32(col)} Вт"; col++;
            }
            else if (typeName.Contains("оперативн") || typeName.Contains("ram") || typeName.Contains("память"))
            {
                specs["Тип"] = rdr.IsDBNull(col) ? "—" : rdr.GetString(col); col++;
                specs["Объём"] = rdr.IsDBNull(col) ? "—" : $"{rdr.GetInt32(col)} ГБ"; col++;
                specs["Количество"] = rdr.IsDBNull(col) ? "—" : $"{rdr.GetInt32(col)} шт."; col++;
                specs["Частота"] = rdr.IsDBNull(col) ? "—" : $"{rdr.GetInt32(col)} МГц"; col++;
                specs["Тайминги"] = rdr.IsDBNull(col) ? "—" : rdr.GetString(col); col++;
            }
            else if (typeName.Contains("материнск") || typeName.Contains("motherboard"))
            {
                specs["Сокет"] = rdr.IsDBNull(col) ? "—" : rdr.GetString(col); col++;
                specs["Форм-фактор"] = rdr.IsDBNull(col) ? "—" : rdr.GetString(col); col++;
                specs["Тип памяти"] = rdr.IsDBNull(col) ? "—" : rdr.GetString(col); col++;
                specs["Слотов памяти"] = rdr.IsDBNull(col) ? "—" : $"{rdr.GetInt32(col)} шт."; col++;
                specs["PCIe слотов"] = rdr.IsDBNull(col) ? "—" : $"{rdr.GetInt32(col)}"; col++;
                specs["SATA портов"] = rdr.IsDBNull(col) ? "—" : $"{rdr.GetInt32(col)}"; col++;
            }
            else if (typeName.Contains("блок питания") || typeName.Contains("psu") || typeName.Contains("power"))
            {
                specs["Мощность"] = rdr.IsDBNull(col) ? "—" : $"{rdr.GetInt32(col)} Вт"; col++;
                specs["Сертификат"] = rdr.IsDBNull(col) ? "—" : rdr.GetString(col); col++;
                specs["Вентилятор"] = rdr.IsDBNull(col) ? "—" : rdr.GetString(col); col++;
            }
            else if (typeName.Contains("кулер") || typeName.Contains("cooler"))
            {
                specs["Вентилятор"] = rdr.IsDBNull(col) ? "—" : rdr.GetString(col); col++;
                specs["Тепловых трубок"] = rdr.IsDBNull(col) ? "—" : $"{rdr.GetInt32(col)}"; col++;
                specs["Мин. скорость"] = rdr.IsDBNull(col) ? "—" : $"{rdr.GetInt32(col)} об/мин"; col++;
                specs["Макс. скорость"] = rdr.IsDBNull(col) ? "—" : $"{rdr.GetInt32(col)} об/мин"; col++;
                specs["Уровень шума"] = rdr.IsDBNull(col) ? "—" : $"{rdr.GetDouble(col):F1} дБ"; col++;
            }
            else if (typeName.Contains("корпус") || typeName.Contains("case"))
            {
                specs["Размер"] = rdr.IsDBNull(col) ? "—" : rdr.GetString(col); col++;
                specs["Отсеков"] = rdr.IsDBNull(col) ? "—" : $"{rdr.GetInt32(col)}"; col++;
                specs["Мест под вентиляторы"] = rdr.IsDBNull(col) ? "—" : $"{rdr.GetInt32(col)}"; col++;
            }
            else if (typeName.Contains("жёсткий") || typeName.Contains("hdd"))
            {
                specs["Объём"] = rdr.IsDBNull(col) ? "—" : $"{rdr.GetInt32(col)} ГБ"; col++;
                specs["Интерфейс"] = rdr.IsDBNull(col) ? "—" : rdr.GetString(col); col++;
                specs["Скорость вращения"] = rdr.IsDBNull(col) ? "—" : $"{rdr.GetInt32(col)} об/мин"; col++;
            }
            else if (typeName.Contains("ssd") || typeName.Contains("накопитель"))
            {
                specs["Объём"] = rdr.IsDBNull(col) ? "—" : $"{rdr.GetInt32(col)} ГБ"; col++;
                specs["Интерфейс"] = rdr.IsDBNull(col) ? "—" : rdr.GetString(col); col++;
                specs["TBW"] = rdr.IsDBNull(col) ? "—" : $"{rdr.GetInt32(col)} ТБ"; col++;
            }
        }
        catch { /* Если данных нет — игнорируем */ }
        return specs;
    }

    // ─── Проверки совместимости ─────────────────────────────────────────────────

    /// <summary>Возвращает список предупреждений о несовместимости</summary>
    public static List<string> CheckCompatibility(Dictionary<string, Part> build)
    {
        var warnings = new List<string>();

        var cpu = GetBuildPart(build, "процессор", "cpu");
        var mb = GetBuildPart(build, "материнск", "motherboard");
        var cooler = GetBuildPart(build, "кулер", "cooler");
        var ram = GetBuildPart(build, "оперативн", "ram", "память");
        var gpu = GetBuildPart(build, "видеокарт", "gpu");
        var psu = GetBuildPart(build, "блок питания", "psu", "power");
        var @case = GetBuildPart(build, "корпус", "case");

        // 1. Совместимость сокета CPU ↔ материнская плата
        if (cpu != null && mb != null)
        {
            if (!CheckCpuMotherboard(cpu.Id, mb.Id))
                warnings.Add("⚠ Сокет процессора не совпадает с сокетом материнской платы");
        }

        // 2. Совместимость сокета CPU ↔ кулер
        if (cpu != null && cooler != null)
        {
            if (!CheckCoolerSocket(cpu.Id, cooler.Id))
                warnings.Add("⚠ Кулер не поддерживает сокет процессора");
        }

        // 3. Совместимость сокета материнской платы ↔ кулер (дополнительно)
        if (mb != null && cooler != null && cpu == null)
        {
            if (!CheckCoolerMotherboard(mb.Id, cooler.Id))
                warnings.Add("⚠ Кулер не поддерживает сокет материнской платы");
        }

        // 4. Форм-фактор материнской платы ↔ корпус
        if (mb != null && @case != null)
        {
            if (!CheckCaseMotherboard(mb.Id, @case.Id))
                warnings.Add("⚠ Форм-фактор материнской платы не поддерживается корпусом");
        }

        // 5. Тип памяти RAM ↔ материнская плата
        if (ram != null && mb != null)
        {
            if (!CheckRamMotherboard(ram.Id, mb.Id))
                warnings.Add("⚠ Тип оперативной памяти не совместим с материнской платой");
        }

        // 6. Мощность БП ≥ рекомендуемая мощность GPU
        if (psu != null && gpu != null)
        {
            if (!CheckPsuGpu(psu.Id, gpu.Id))
                warnings.Add("⚠ Мощности блока питания недостаточно для видеокарты");
        }

        return warnings;
    }

    private static Part? GetBuildPart(Dictionary<string, Part> build, params string[] keywords)
    {
        foreach (var kv in build)
        {
            var key = kv.Key.ToLower();
            if (keywords.Any(k => key.Contains(k)))
                return kv.Value;
        }
        return null;
    }

    private static bool CheckCpuMotherboard(int cpuId, int mbId)
    {
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(
            "SELECT COUNT(*) FROM cpu$ c JOIN motherboard$ m ON c.socketid = m.socketid WHERE c.id=@cpu AND m.id=@mb",
            conn);
        cmd.Parameters.AddWithValue("@cpu", cpuId);
        cmd.Parameters.AddWithValue("@mb", mbId);
        return (int)cmd.ExecuteScalar()! > 0;
    }

    private static bool CheckCoolerSocket(int cpuId, int coolerId)
    {
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(
            @"SELECT COUNT(*) FROM socketprocessorcooler$ spc
              JOIN cpu$ c ON c.socketid = spc.socketid
              WHERE c.id = @cpu AND spc.processorcoolerid = @cooler",
            conn);
        cmd.Parameters.AddWithValue("@cpu", cpuId);
        cmd.Parameters.AddWithValue("@cooler", coolerId);
        return (int)cmd.ExecuteScalar()! > 0;
    }

    private static bool CheckCoolerMotherboard(int mbId, int coolerId)
    {
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(
            @"SELECT COUNT(*) FROM socketprocessorcooler$ spc
              JOIN motherboard$ m ON m.socketid = spc.socketid
              WHERE m.id = @mb AND spc.processorcoolerid = @cooler",
            conn);
        cmd.Parameters.AddWithValue("@mb", mbId);
        cmd.Parameters.AddWithValue("@cooler", coolerId);
        return (int)cmd.ExecuteScalar()! > 0;
    }

    private static bool CheckCaseMotherboard(int mbId, int caseId)
    {
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(
            @"SELECT COUNT(*) FROM boardformfactorcase$ bfc
              JOIN motherboard$ m ON m.formfactorid = bfc.formfactorid
              WHERE m.id = @mb AND bfc.caseid = @case",
            conn);
        cmd.Parameters.AddWithValue("@mb", mbId);
        cmd.Parameters.AddWithValue("@case", caseId);
        return (int)cmd.ExecuteScalar()! > 0;
    }

    private static bool CheckRamMotherboard(int ramId, int mbId)
    {
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(
            "SELECT COUNT(*) FROM ram$ r JOIN motherboard$ m ON r.memorytypeid = m.memorytypeid WHERE r.id=@ram AND m.id=@mb",
            conn);
        cmd.Parameters.AddWithValue("@ram", ramId);
        cmd.Parameters.AddWithValue("@mb", mbId);
        return (int)cmd.ExecuteScalar()! > 0;
    }

    private static bool CheckPsuGpu(int psuId, int gpuId)
    {
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(
            "SELECT COUNT(*) FROM powersupply$ ps, gpu$ g WHERE ps.id=@psu AND g.id=@gpu AND (g.recommendpower IS NULL OR ps.power >= g.recommendpower)",
            conn);
        cmd.Parameters.AddWithValue("@psu", psuId);
        cmd.Parameters.AddWithValue("@gpu", gpuId);
        return (int)cmd.ExecuteScalar()! > 0;
    }

    // ─── Сохранение/загрузка сборок ─────────────────────────────────────────────

    public static void SaveAssembly(string name, string author, List<int> partIds)
    {
        using var conn = OpenConnection();
        using var tran = conn.BeginTransaction();
        try
        {
            // Сохраняем сборку
            using var cmdInsert = new SqlCommand(
                "INSERT INTO assembly$ (name, author) OUTPUT INSERTED.id VALUES (@name, @author)",
                conn, tran);
            cmdInsert.Parameters.AddWithValue("@name", name);
            cmdInsert.Parameters.AddWithValue("@author", author);
            int assemblyId = (int)cmdInsert.ExecuteScalar()!;

            // Сохраняем комплектующие
            foreach (int partId in partIds)
            {
                using var cmdPart = new SqlCommand(
                    "INSERT INTO partassembly$ (partid, assemblyid) VALUES (@partId, @assemblyId)",
                    conn, tran);
                cmdPart.Parameters.AddWithValue("@partId", partId);
                cmdPart.Parameters.AddWithValue("@assemblyId", assemblyId);
                cmdPart.ExecuteNonQuery();
            }

            tran.Commit();
        }
        catch
        {
            tran.Rollback();
            throw;
        }
    }

    public static List<SavedBuild> GetAllAssemblies()
    {
        var builds = new Dictionary<int, SavedBuild>();

        using var conn = OpenConnection();
        using var cmd = new SqlCommand(
            @"SELECT a.id, a.name, a.author,
                     bp.id, bp.name, m.name, bp.manufacturerid, bp.parttypeid,
                     bp.image, CAST(bp.price AS DECIMAL(18,2)), pt.name
              FROM assembly$ a
              LEFT JOIN partassembly$ pa ON pa.assemblyid = a.id
              LEFT JOIN basepart$ bp ON bp.id = pa.partid
              LEFT JOIN manufacturer$ m ON m.id = bp.manufacturerid
              LEFT JOIN parttype$ pt ON pt.id = bp.parttypeid
              ORDER BY a.id, pt.name",
            conn);

        using var rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            int aId = rdr.GetInt32(0);
            if (!builds.ContainsKey(aId))
                builds[aId] = new SavedBuild
                {
                    Id = aId,
                    Name = rdr.GetString(1),
                    Author = rdr.GetString(2)
                };

            if (!rdr.IsDBNull(3))
            {
                builds[aId].Parts.Add(new Part
                {
                    Id = rdr.GetInt32(3),
                    Name = rdr.GetString(4),
                    Manufacturer = rdr.IsDBNull(5) ? "" : rdr.GetString(5),
                    ManufacturerId = rdr.IsDBNull(6) ? 0 : rdr.GetInt32(6),
                    PartTypeId = rdr.IsDBNull(7) ? 0 : rdr.GetInt32(7),
                    Image = rdr.IsDBNull(8) ? "" : rdr.GetString(8),
                    Price = rdr.IsDBNull(9) ? 0 : rdr.GetDecimal(9),
                    PartTypeName = rdr.IsDBNull(10) ? "" : rdr.GetString(10)
                });
            }
        }

        return builds.Values.ToList();
    }

    public static void DeleteAssembly(int assemblyId)
    {
        using var conn = OpenConnection();
        using var cmd = new SqlCommand(
            "DELETE FROM partassembly$ WHERE assemblyid=@id; DELETE FROM assembly$ WHERE id=@id",
            conn);
        cmd.Parameters.AddWithValue("@id", assemblyId);
        cmd.ExecuteNonQuery();
    }
}
