# Avalonia-Face-Metadata-Extractor
Extract Face Metadata with MediaPipe with Google Jules and Claude code and Gemini

## 개요
AvaloniaUI 데스크톱 애플리케이션에서 MediaPipe를 사용하여 얼굴의 각종 메타데이터를 추출합니다.
목표: MediaPipe에서 감지하는 랜드마크(눈, 코, 입 등), 표정, 머리 회전율과 같은 기본적인 정보를 반드시 포함하며, 형태학적 분석, 기타 딥러닝 추론 모델(HugginFace 등)의 도움을 받아 다수 인원의 성비, 인종 등의 고급 메타데이터까지 추출하는 것입니다. 최종적으로 분석된 데이터를 저장, 조회할 수 있는 기능을 구현합니다.

---

## 제약사항 & 준수사항
- .Net8 환경에서 가능한 한 최신 C# 문법들을 사용합니다.
- MediaPipe와 OpenCvSharp4는 반드시 포함합니다.
- HugginFace 모델 연동을 위해 Onnx 라이브러리 사용을 검토합니다.
- 데이터 저장을 위해 초기에는 SQLite RDB를 사용하며, Face, Metadata 테이블 스키마를 포함한 간단한 데이터 모델을 구성합니다.
- CommunityToolkits.Mvvm 및 Microsoft.Extensions.Hosting 등을 사용하여 정석적인 MVVM 아키텍처를 구현합니다.
- 테스트 프로젝트는 XUnit을 기준으로 합니다.
- 솔루션(.sln) 내부에 단일 CsProject가 아닌 목적에 맞는 여러 프로젝트들을 Layered 아키텍처 스타일로 배치합니다. 테스트가 가능하도록 UI와 로직의 분리에 집중합니다. (예: `MetaExtractor.App.UI.Avalonia` (시작 프로젝트), `MetaExtractor.Core`, `MetaExtractor.Domain` 등)
- 테스트 커버리지를 측정하고 GitPages에 게시하는 Git Actions 워크플로가 구성되어야 합니다.
- MediaPipe 외에 목적을 달성하기 위한 라이브러리들을 자유롭게 구성해도 좋습니다. 하지만 구성과 라이브러리들 간 호환성을 고려해야 합니다.
- Git Branching Workflow:
  - 기능 작업 시작 시 Branch를 생성합니다. (예: `feature/analyze-view`)
  - 수정 간에 빌드 확인하면서 Commit합니다.
  - 기능 작업 완료 후 Pull Request(PR)를 생성합니다.
  - 기능 추가 시 `main` branch에서 직접 작업하는 것을 지양합니다.

---

## 아키텍처 및 디자인 패턴
- 종속성 주입 (DI): Microsoft.Extensions.Hosting을 사용하여 서비스 컨테이너를 구성하고, `IServiceCollection`을 통해 애플리케이션 전반의 종속성(ViewModel, Service 등)을 관리합니다. 이는 UI와 로직을 분리하고 단위 테스트를 용이하게 하기 위함입니다.
- 상태 패턴 (State Pattern): 이미지 처리 상태(예: Idle, Loading, Analyzing, Completed)를 관리하는 `IImageProcessingState` 인터페이스를 정의하고, 상태 전환이 자연스럽게 이루어지도록 구현합니다.
- 전략 패턴 (Strategy Pattern): 이미지 소스(카메라 라이브 스트림, 로컬 파일)에 따라 데이터를 가져오는 방법을 유연하게 교체할 수 있도록 `IImageSourceStrategy` 인터페이스를 활용합니다.
- 팩토리 패턴 (Factory Pattern): 분석 모델(예: 기본 모델, 고급 모델)을 생성하는 로직을 캡슐화하기 위해 `IModelFactory` 인터페이스를 사용합니다.

---

## 필수 구현 기능
- 쉘(Shell) 스타일 UI: 전체 애플리케이션의 뼈대 역할을 하는 `ShellView`를 구성하고, 네비게이션을 통해 Analyze, Data Cluster, Settings 페이지로 이동할 수 있는 구조를 만듭니다.
- 이미지 로딩 혹은 카메라 라이브 이미지를 연동하는 기능
- 메타데이터 추출 및 표시를 위한 `AnalyzeView` 및 `AnalyzeViewModel`, 그리고 관련 Model들
- 메타데이터 원시 데이터에 대한 군집 구성, 테스트, 조회 기능을 가지는 `DataClusterView`, `DataClusterViewModel`, 관련 Model들
- 모던 대시보드 스타일의 UI 구현:
  - 사이드바 네비게이션: 좌측에 Analyze (분석), Data Cluster (데이터 군집), Settings (설정) 등 주요 기능으로 이동하는 네비게이션 메뉴를 구성합니다. 각 메뉴 아이템은 아이콘과 텍스트를 포함합니다.
  - 대시보드 레이아웃: 각 페이지는 메인 콘텐츠 영역을 가지며, 상단에 페이지 제목과 부가 정보를 표시합니다.
  - Settings 페이지: 사용자가 카메라 설정, 데이터 저장 경로, 딥러닝 모델 설정 등을 변경할 수 있는 UI를 포함합니다.
  - 디자인 가이드라인: 깔끔하고 미니멀한 디자인을 지향합니다. 다크 모드를 기본으로 고려하거나 선택 옵션으로 제공하는 것이 좋습니다.