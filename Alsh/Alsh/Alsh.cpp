#include <windows.h>
#include <iostream>
#include <string>


int main(int argc, char* argv[]) {
    if (argc < 3) {
        std::cerr << "Usage: program <filename> <count>" << std::endl;
        return -1;
    }
    
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);

    std::string inputFile = argv[1];
    int maxCount = atoi(argv[2]);
    std::string outputFile = inputFile;

    HANDLE hInput = CreateFileA(inputFile.c_str(), GENERIC_READ, 0, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
    if (hInput == INVALID_HANDLE_VALUE) {
        std::cerr << "Файл не найден" << std::endl;
        return -1;
    }

    DWORD inputSize = GetFileSize(hInput, NULL);
    DWORD outputSize = inputSize * 2;

    HANDLE hInputMap = CreateFileMappingA(hInput, NULL, PAGE_READONLY, 0, 0, NULL);
    if (!hInputMap) {
        CloseHandle(hInput);
        return -1;
    }

    char* inputData = (char*)MapViewOfFile(hInputMap, FILE_MAP_READ, 0, 0, 0);
    if (!inputData) {
        CloseHandle(hInputMap);
        CloseHandle(hInput);
        return -1;
    }

    HANDLE hOutput = CreateFileA(outputFile.c_str(), GENERIC_READ | GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
    if (hOutput == INVALID_HANDLE_VALUE) {
        std::cerr << "Невозможно создать выходной файл" << std::endl;
        UnmapViewOfFile(inputData);
        CloseHandle(hInputMap);
        CloseHandle(hInput);
        return -1;
    }

    if (SetFilePointer(hOutput, outputSize, NULL, FILE_BEGIN) == INVALID_SET_FILE_POINTER || !SetEndOfFile(hOutput)) {
        std::cerr << "Невозможно изменить размер файла" << std::endl;
        CloseHandle(hOutput);
        UnmapViewOfFile(inputData);
        CloseHandle(hInputMap);
        CloseHandle(hInput);
        return -1;
    }

    HANDLE hOutputMap = CreateFileMappingA(hOutput, NULL, PAGE_READWRITE, 0, 0, NULL);
    if (!hOutputMap) {
        CloseHandle(hOutput);
        UnmapViewOfFile(inputData);
        CloseHandle(hInputMap);
        CloseHandle(hInput);
        return -1;
    }

    wchar_t* outputData = (wchar_t*)MapViewOfFile(hOutputMap, FILE_MAP_WRITE, 0, 0, 0);
    if (!outputData) {
        CloseHandle(hOutputMap);
        CloseHandle(hOutput);
        UnmapViewOfFile(inputData);
        CloseHandle(hInputMap);
        CloseHandle(hInput);
        return -1;
    }

    int count = 0;
    for (DWORD i = 0; i < inputSize; i++) {
        unsigned char c = (unsigned char)inputData[i];
        wchar_t wc = (wchar_t)c;

        if (count < maxCount) {
            if (c >= 0xE0 && c <= 0xFF) {
                wc = (wchar_t)(c - 0x20);
                count++;
            }
            else if (c == 0xB8) {
                wc = (wchar_t)0xA8;
                count++;
            }
        }

        outputData[i] = wc;
    }

    UnmapViewOfFile(outputData);
    UnmapViewOfFile(inputData);
    CloseHandle(hOutputMap);
    CloseHandle(hOutput);
    CloseHandle(hInputMap);
    CloseHandle(hInput);

    std::cout << count << std::endl;
    return count;
}