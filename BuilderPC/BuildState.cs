using System;
using System.Collections.Generic;
using System.Linq;
using BuilderPC.Data;

namespace BuilderPC
{
    /// <summary>
    /// Статический класс — текущая сборка пользователя.
    /// Ключ словаря — id типа комплектующего (parttypeid).
    /// </summary>
    public static class BuildState
    {
        // partTypeId -> basepart_
        private static Dictionary<int, basepart_> _selected = new Dictionary<int, basepart_>();

        public static event Action Changed;

        public static void Set(basepart_ part)
        {
            _selected[part.parttypeid] = part;
            Changed?.Invoke();
        }

        public static void Remove(int partTypeId)
        {
            _selected.Remove(partTypeId);
            Changed?.Invoke();
        }

        public static void Clear()
        {
            _selected.Clear();
            Changed?.Invoke();
        }

        public static IEnumerable<basepart_> GetAll() => _selected.Values;

        public static decimal GetTotal() => _selected.Values.Sum(p => (decimal)p.price);

        public static List<int> GetPartIds() => _selected.Values.Select(p => p.id).ToList();

        public static bool HasType(int partTypeId) => _selected.ContainsKey(partTypeId);

        // ── Проверка совместимости ────────────────────────────────────────

        public static List<string> GetCompatibilityIssues()
        {
            var issues = new List<string>();

            // Находим выбранные комплектующие по таблицам
            basepart_ cpuPart        = _selected.Values.FirstOrDefault(p => Core.Context.cpu_.Any(c => c.id == p.id));
            basepart_ gpuPart        = _selected.Values.FirstOrDefault(p => Core.Context.gpu_.Any(g => g.id == p.id));
            basepart_ mbPart         = _selected.Values.FirstOrDefault(p => Core.Context.motherboard_.Any(m => m.id == p.id));
            basepart_ ramPart        = _selected.Values.FirstOrDefault(p => Core.Context.ram_.Any(r => r.id == p.id));
            basepart_ casePart       = _selected.Values.FirstOrDefault(p => Core.Context.case_.Any(c => c.id == p.id));
            basepart_ psuPart        = _selected.Values.FirstOrDefault(p => Core.Context.powersupply_.Any(ps => ps.id == p.id));
            basepart_ coolerPart     = _selected.Values.FirstOrDefault(p => Core.Context.processorcooler_.Any(pc => pc.id == p.id));

            cpu_            cpu    = cpuPart    != null ? Core.Context.cpu_.FirstOrDefault(c => c.id == cpuPart.id)       : null;
            gpu_            gpu    = gpuPart    != null ? Core.Context.gpu_.FirstOrDefault(g => g.id == gpuPart.id)       : null;
            motherboard_    mb     = mbPart     != null ? Core.Context.motherboard_.FirstOrDefault(m => m.id == mbPart.id) : null;
            ram_            ram    = ramPart    != null ? Core.Context.ram_.FirstOrDefault(r => r.id == ramPart.id)       : null;
            case_           cas    = casePart   != null ? Core.Context.case_.FirstOrDefault(c => c.id == casePart.id)    : null;
            powersupply_    psu    = psuPart    != null ? Core.Context.powersupply_.FirstOrDefault(ps => ps.id == psuPart.id) : null;
            processorcooler_ cooler = coolerPart != null ? Core.Context.processorcooler_.FirstOrDefault(pc => pc.id == coolerPart.id) : null;

            // 1. Сокет: ЦП ↔ Материнская плата
            if (cpu != null && mb != null && cpu.socketid != mb.socketid)
            {
                string cpuSocket = Core.Context.socket_.FirstOrDefault(s => s.id == cpu.socketid)?.name ?? "?";
                string mbSocket  = Core.Context.socket_.FirstOrDefault(s => s.id == mb.socketid)?.name  ?? "?";
                issues.Add($"Сокет ЦП ({cpuSocket}) не совпадает с сокетом материнской платы ({mbSocket})");
            }

            // 2. Сокет: ЦП ↔ Кулер
            if (cpu != null && cooler != null)
            {
                bool coolerOk = Core.Context.socketprocessorcooler_.Any(
                    spc => spc.processorcoolerid == cooler.id && spc.socketid == cpu.socketid);
                if (!coolerOk)
                {
                    string cpuSocket = Core.Context.socket_.FirstOrDefault(s => s.id == cpu.socketid)?.name ?? "?";
                    var supportedIds = Core.Context.socketprocessorcooler_
                        .Where(spc => spc.processorcoolerid == cooler.id)
                        .Select(spc => spc.socketid).ToList();
                    string supported = string.Join(", ", Core.Context.socket_
                        .Where(s => supportedIds.Contains(s.id)).Select(s => s.name));
                    issues.Add($"Кулер не поддерживает сокет {cpuSocket} (поддерживает: {supported})");
                }
            }

            // 3. Форм-фактор: Материнская плата ↔ Корпус
            if (mb != null && cas != null)
            {
                bool caseOk = Core.Context.boardformfactorcase_.Any(
                    bfc => bfc.caseid == cas.id && bfc.formfactorid == mb.formfactorid);
                if (!caseOk)
                {
                    string mbFf = Core.Context.formfactor_.FirstOrDefault(f => f.id == mb.formfactorid)?.name ?? "?";
                    var suppIds = Core.Context.boardformfactorcase_
                        .Where(bfc => bfc.caseid == cas.id).Select(bfc => bfc.formfactorid).ToList();
                    string suppFf = string.Join(", ", Core.Context.formfactor_
                        .Where(f => suppIds.Contains(f.id)).Select(f => f.name));
                    issues.Add($"Корпус не поддерживает форм-фактор {mbFf} (поддерживает: {suppFf})");
                }
            }

            // 4. Тип памяти: ОЗУ ↔ Материнская плата
            if (ram != null && mb != null && ram.memorytypeid != mb.memorytypeid)
            {
                string ramMt = Core.Context.memorytype_.FirstOrDefault(m => m.id == ram.memorytypeid)?.name ?? "?";
                string mbMt  = Core.Context.memorytype_.FirstOrDefault(m => m.id == mb.memorytypeid)?.name  ?? "?";
                issues.Add($"Тип памяти ОЗУ ({ramMt}) не совместим с материнской платой ({mbMt})");
            }

            // 5. Мощность БП ↔ Рекомендация видеокарты
            if (gpu != null && psu != null && gpu.recommendpower != null)
            {
                if (psu.power < gpu.recommendpower.Value)
                    issues.Add($"Мощность БП ({psu.power} Вт) меньше рекомендованной для видеокарты ({gpu.recommendpower} Вт)");
            }

            return issues;
        }
    }
}
