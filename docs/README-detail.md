# Book Rental Management System

ASP.NET Core Web API와 MySQL을 기반으로 도서 대여 업무를 처리하고,  
HTML/CSS/JavaScript 웹 관리 화면을 연동한 도서 대여 관리 시스템입니다.

회원·도서·도서 분류 정보를 관리하며, 도서 대여 및 반납 처리,
대여 가능 여부 확인, 현재 대여 현황 및 이력 조회 기능을 구현했습니다.

단순 CRUD뿐만 아니라 대여 중인 도서의 중복 대여 방지와
회원·도서 정보를 JOIN한 상세 대여 이력 조회 등
실제 대여 업무 흐름을 기준으로 기능을 구성했습니다.

<br>

## 주요 화면

### 대여 현황 및 반납 관리

![대여 현황 및 반납 관리](./images/book-rental-dashboard.png)

### 도서 대여 및 이력 조회

![도서 대여 및 이력 조회](./images/book-rental-history.png)

웹 관리 화면에서 다음 기능을 사용할 수 있습니다.

- 전체 도서 수 및 대여 현황 조회
- 현재 대여 중인 도서 조회
- 회원과 대여 가능한 도서를 선택하여 대여 처리
- 현재 대여 중인 도서 반납 처리
- 전체 대여 이력 조회
- 도서명 및 회원명 기반 이력 검색
- 최근 5건 / 전체 이력 전환
- 대여·반납 후 화면 데이터 자동 갱신

<br>

## ERD

도서 분류, 도서, 회원, 대여 이력 간의 관계를 기준으로 데이터베이스를 구성했습니다.

![ERD](./images/erd.png)

<br>

## 사용 기술

### Backend

- C#
- ASP.NET Core Web API
- MySqlConnector

### Frontend

- HTML
- CSS
- JavaScript
- Fetch API

### Database

- MySQL

### Development / Test

- Visual Studio
- Postman
- Docker

<br>

## 주요 기능

### 도서 및 회원 관리

- 도서 분류 CRUD
- 도서 CRUD
- 회원 CRUD
- 대여 가능한 도서 목록 조회

### 대여 관리

- 도서 대여 처리
- 도서 반납 처리
- 현재 대여 중인 도서 조회
- 이미 대여 중인 도서의 중복 대여 방지

### 대여 이력

- 전체 대여 이력 조회
- 회원별 대여 이력 조회
- 도서별 대여 이력 조회
- 회원명·도서명이 포함된 상세 이력 조회
- 웹 화면에서 도서명·회원명 검색
- 최근 5건 / 전체 이력 조회

### 웹 관리 화면

- 대여 현황 요약
- 회원 및 대여 가능 도서 선택
- 대여·반납 처리
- 상태별 UI 표시
- 대여/반납 후 현황 및 이력 자동 갱신

<br>

## 시스템 흐름

```text
사용자
  │
  ▼
HTML / CSS / JavaScript
  │
  │ Fetch API
  ▼
ASP.NET Core Web API
  │
  │ SQL
  ▼
MySQL
```

대여 및 반납 처리 후 프론트엔드에서 API를 다시 호출하여
현재 대여 현황, 대여 가능한 도서 및 대여 이력을 자동으로 갱신합니다.

<br>

## 프로젝트 구조

```text
BookRentalApi
 ┣ Controllers
 ┃ ┣ BooksController.cs
 ┃ ┣ DivisionsController.cs
 ┃ ┣ MembersController.cs
 ┃ ┗ RentalsController.cs
 ┣ Models
 ┃ ┣ Book.cs
 ┃ ┣ BookRequest.cs
 ┃ ┣ Division.cs
 ┃ ┣ Member.cs
 ┃ ┣ MemberRequest.cs
 ┃ ┣ Rental.cs
 ┃ ┣ RentalRequest.cs
 ┃ ┗ RentalDetail.cs
 ┣ wwwroot
 ┃ ┣ css
 ┃ ┃ ┗ style.css
 ┃ ┣ js
 ┃ ┃ ┗ app.js
 ┃ ┗ index.html
 ┣ Properties
 ┃ ┗ launchSettings.json
 ┣ Program.cs
 ┣ appsettings.json
 ┗ Dockerfile
```

<br>

## API 목록

### 도서 분류 API

| Method | URL | 설명 |
|---|---|---|
| GET | `/api/divisions` | 도서 분류 목록 조회 |
| GET | `/api/divisions/{divCode}` | 도서 분류 단건 조회 |
| POST | `/api/divisions` | 도서 분류 등록 |
| PUT | `/api/divisions/{divCode}` | 도서 분류 수정 |
| DELETE | `/api/divisions/{divCode}` | 도서 분류 삭제 |

### 도서 API

| Method | URL | 설명 |
|---|---|---|
| GET | `/api/books` | 도서 목록 조회 |
| GET | `/api/books/{id}` | 도서 단건 조회 |
| GET | `/api/books/available` | 대여 가능한 도서 조회 |
| POST | `/api/books` | 도서 등록 |
| PUT | `/api/books/{id}` | 도서 수정 |
| DELETE | `/api/books/{id}` | 도서 삭제 |

### 회원 API

| Method | URL | 설명 |
|---|---|---|
| GET | `/api/members` | 회원 목록 조회 |
| GET | `/api/members/{id}` | 회원 단건 조회 |
| POST | `/api/members` | 회원 등록 |
| PUT | `/api/members/{id}` | 회원 수정 |
| DELETE | `/api/members/{id}` | 회원 삭제 |

### 대여 API

| Method | URL | 설명 |
|---|---|---|
| GET | `/api/rentals` | 전체 대여 이력 조회 |
| GET | `/api/rentals/{id}` | 대여 이력 단건 조회 |
| GET | `/api/rentals/active` | 현재 대여 중인 목록 조회 |
| GET | `/api/rentals/details` | 상세 대여 이력 조회 |
| GET | `/api/rentals/active/details` | 현재 대여 중인 상세 목록 조회 |
| GET | `/api/rentals/member/{memberId}` | 회원별 대여 이력 조회 |
| GET | `/api/rentals/member/{memberId}/details` | 회원별 상세 이력 조회 |
| GET | `/api/rentals/book/{bookId}` | 도서별 대여 이력 조회 |
| GET | `/api/rentals/book/{bookId}/details` | 도서별 상세 이력 조회 |
| POST | `/api/rentals` | 도서 대여 |
| PUT | `/api/rentals/{id}/return` | 도서 반납 |

<br>

## 주요 기능 흐름

### 도서 대여

도서 대여 요청 시 단순히 대여 이력을 등록하지 않고
회원과 도서의 존재 여부 및 현재 대여 상태를 확인합니다.

```text
1. 회원 존재 여부 확인
2. 도서 존재 여부 확인
3. 현재 대여 중인 도서인지 확인
4. 대여 가능한 경우 대여 이력 등록
5. 이미 대여 중인 경우 요청 차단
```

요청:

```http
POST /api/rentals
```

```json
{
  "memberIdx": 1,
  "bookIdx": 3
}
```

<br>

### 중복 대여 방지

반납되지 않은 도서가 다시 대여되는 것을 방지합니다.

```sql
SELECT COUNT(*)
FROM rentals
WHERE book_idx = @BookIdx
  AND returnDate IS NULL;
```

대여 중인 도서인 경우 대여 요청을 처리하지 않습니다.

<br>

### 도서 반납

```http
PUT /api/rentals/{id}/return
```

반납되지 않은 대여 내역에 대해 `returnDate`를 현재 날짜로 변경합니다.

```text
1. 대여 번호 확인
2. 반납되지 않은 대여인지 확인
3. 반납일 저장
4. 대여 가능 도서 목록에 다시 반영
```

<br>

### 상세 대여 이력

대여 테이블의 회원 번호와 도서 번호만 반환하지 않고,
회원 및 도서 테이블을 JOIN하여 실제 화면에서 필요한 정보를 제공합니다.

```http
GET /api/rentals/details
```

응답 예시:

```json
[
  {
    "rentalIdx": 1,
    "memberIdx": 1,
    "memberName": "김민수",
    "bookIdx": 3,
    "bookName": "ASP.NET Core Web API 입문",
    "rentalDate": "2026-06-30T00:00:00",
    "returnDate": null,
    "status": "대여중"
  }
]
```

<br>

## 프론트엔드 API 연동

웹 관리 화면은 JavaScript Fetch API를 사용하여
ASP.NET Core Web API와 통신합니다.

초기 화면 로딩 시 다음 데이터를 비동기로 조회합니다.

```text
도서 목록
대여 가능한 도서
현재 대여 목록
회원 목록
전체 대여 이력
```

대여 또는 반납 처리 후 관련 데이터를 다시 조회하여
페이지 전체를 새로고침하지 않고 화면 상태를 갱신합니다.

또한 반복되는 데이터 갱신 로직과 대여 이력 검색 로직을
공통 함수로 분리하여 JavaScript 코드의 중복을 줄였습니다.

<br>

## Postman 테스트

웹 화면 구현 전후로 Postman을 사용하여
각 REST API의 동작과 대여 업무 흐름을 확인했습니다.

```text
1. 대여 가능 도서 목록 조회
2. 도서 대여
3. 현재 대여 목록 확인
4. 대여 가능 목록에서 해당 도서 제외 확인
5. 동일 도서 재대여 시도
6. 중복 대여 차단 확인
7. 도서 반납
8. 대여 가능 목록 재등록 확인
```

### 도서 대여 성공

![도서 대여 성공](./images/rent-book-success.png)

### 중복 대여 차단

![중복 대여 실패](./images/rent-book-duplicate-fail.png)

### 도서 반납

![도서 반납 성공](./images/return-book-success.png)

### 상세 대여 이력

![대여 상세 조회](./images/rental-details.png)

<br>

## Docker

ASP.NET Core Web API를 Docker 이미지로 빌드하여
컨테이너 환경에서도 실행할 수 있도록 구성했습니다.

### 이미지 빌드

```powershell
docker build -t bookrentalapi .
```

### 컨테이너 실행

```powershell
docker run -d --name bookrentalapi-container -p 9090:8080 bookrentalapi
```

```text
localhost:9090 → container:8080
```

### 실행 확인

```powershell
docker ps
```

![Docker 실행 화면](./images/docker-container.png)

<br>

## 구현 및 학습 내용

- ASP.NET Core Web API 기반 REST API 구현
- MySqlConnector를 활용한 MySQL 연동
- 비동기 DB 처리
- GET / POST / PUT / DELETE API 구현
- 테이블 관계를 활용한 대여·반납 업무 로직 구현
- 중복 대여 방지 로직 구현
- JOIN을 활용한 상세 응답 데이터 구성
- HTML/CSS/JavaScript 기반 관리 화면 구현
- Fetch API를 활용한 프론트엔드·백엔드 연동
- 대여 및 반납 후 화면 상태 자동 갱신
- JavaScript 공통 함수 분리를 통한 중복 코드 정리
- Docker 기반 실행 환경 구성
