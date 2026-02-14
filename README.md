# George-Kimi-repo

AI 시니어 개발자 **죠지(George)**의 개발 산출물 저장소입니다.

> **⚠️ 이 저장소는 개발 산출물(소스코드) 저장소입니다.**  
> 프로젝트 관리 및 메타 정보는 [George-Kimi](https://github.com/devmcjo/George-Kimi)에서 확인하세요.

---

## 📦 저장소 역할

이 저장소는 죠지(George)가 개발한 **실제 소스코드 및 실행 파일**을 관리하는 저장소입니다.

| 저장소 | 역할 | URL |
|--------|------|-----|
| **George-Kimi** | 프로젝트 관리, 문서, 계획서 | https://github.com/devmcjo/George-Kimi |
| **George-Kimi-repo** (본 저장소) | 개발 산출물 (실제 소스코드) | https://github.com/devmcjo/George-Kimi-repo |

---

## 📁 프로젝트 구조

```
Kimi-repo/
├── README.md              # 본 파일
├── project001/            # TestProject
│   └── ...
├── project002/            # Engineering Calculator (공학용 계산기)
│   └── EngineeringCalculator/
│       ├── EngineeringCalculator.sln
│       └── EngineeringCalculator/
│           ├── MainWindow.xaml
│           ├── MainWindow.xaml.cs
│           └── ...
└── project003/            # WeatherApp (날씨 정보 프로그램)
    └── ...
```

---

## 🚀 프로젝트 목록

### project001 - TestProject
- **설명**: 테스트용 프로젝트
- **기술 스택**: (테스트 목적)
- **실행 파일**: -\n
### project002 - Engineering Calculator
- **설명**: Windows 11용 공학용 계산기
- **기술 스택**: C# (.NET 6), WPF
- **기능**: 사칙연산, 공학용 함수(sin, cos, tan, log, ln, sqrt, pow), 계산 히스토리(20개)
- **실행 파일**: `project002/EngineeringCalculator.exe`
- **빌드 방법**:
  ```powershell
  cd project002/EngineeringCalculator
  dotnet build -c Release
  dotnet publish -c Release --self-contained true -r win-x64 -p:PublishSingleFile=true
  ```

### project003 - WeatherApp
- **설명**: 실행 시점부터 일주일간의 날씨 정보를 보여주는 프로그램
- **기술 스택**: C# (.NET 6/8), WPF
- **API**: OpenWeatherMap 또는 기상청 API
- **실행 파일**: `project003/WeatherApp.exe`
- **상태**: 개발 예정

---

## 🛠️ 빌드 및 실행

### 요구사항

- Windows 10/11
- .NET 6.0 SDK 이상
- **Visual Studio 2022** (권장) 또는 **Visual Studio Code**

### 개발 환경 (IDE)

죠지는 다음 IDE만을 사용하여 개발을 수행합니다:

| IDE | 버전 | 권장 사용처 |
|-----|------|------------|
| **Visual Studio** | 2022 | Windows 데스크톱 앱, C++, C#, MFC, WPF 개발 |
| **Visual Studio Code** | 최신 | 경량 프로젝트, Python, 설정 편집 |

> **정책**: 죠지는 위 두 IDE 외에는 사용하지 않습니다.

### 일반적인 빌드 방법

```powershell
# 프로젝트 폴더로 이동
cd project002/EngineeringCalculator

# Debug 빌드
dotnet build

# Release 빌드
dotnet build -c Release

# 단일 실행 파일로 발행
dotnet publish -c Release --self-contained true -r win-x64 -p:PublishSingleFile=true
```

---

## 📋 Git 정책

이 저장소는 George-Kimi의 개발 산출물을 관리합니다.

- **커밋**: `[George]` prefix 사용 (예: `[George] feat: 기능 추가`)
- **빌드 산출물**: `bin/`, `obj/`, `*.exe` 등은 Git에 포함하지 않음 (.gitignore 참조)
- **발행 파일**: Release 빌드로 생성된 단일 실행 파일은 포함 가능

---

## 🔗 관련 저장소

```
┌─────────────────────────────────────┐
│      George-Kimi (메타 저장소)       │
│  - 문서, 계획서, 설정, 교훈 등 관리   │
│  - https://github.com/devmcjo/...   │
└─────────────────┬───────────────────┘
                  │ 서브모듈 연결
                  ▼
┌─────────────────────────────────────┐
│    George-Kimi-repo (코드 저장소)    │
│  - 실제 개발 산출물(소스코드) 관리    │
│  - https://github.com/devmcjo/...   │
└─────────────────────────────────────┘
```

---

## 👤 개발자

- **AI 개발자**: 죠지 (George) - Kimi 2.5k 기반
- **팀 리드**: 조명철 (mcJo)

---

_Powered by Kimi 2.5k (Moonshot AI)_
