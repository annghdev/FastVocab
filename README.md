# 🚀 FastVocab

FastVocab là một ứng dụng web được xây dựng với .NET 9 và Blazor Server, giúp người dùng học và quản lý từ vựng một cách hiệu quả thông qua các phương pháp luyện tập đa dạng.

![.NET](https://img.shields.io/badge/.NET-9-blueviolet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-blue)
![Blazor Server](https://img.shields.io/badge/Blazor-Server-purple)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-red)
![License](https://img.shields.io/badge/License-MIT-green)
![PRs Welcome](https://img.shields.io/badge/PRs-welcome-blue.svg)

---

## 📋 Mục lục

* [Giới thiệu](#-giới-thiệu)
* [Tính năng chính](#-tính-năng-chính)
* [Demo & Hình ảnh](#-demo--hình-ảnh)
* [Công nghệ & Kiến trúc](#-công-nghệ--kiến-trúc)
* [Hướng dẫn cài đặt](#-hướng-dẫn-cài-đặt)
* [Giấy phép](#-giấy-phép)

---

## 🌟 Giới thiệu

**FastVocab** không chỉ là một ứng dụng ghi chú từ vựng đơn thuần. Đây là một hệ thống toàn diện cho phép bạn:
* Tổ chức từ vựng theo các chủ đề (topics) cụ thể.
* Học từ mới thông qua giao diện người dùng trực quan.
* Ôn tập kiến thức với nhiều hình thức kiểm tra: trắc nghiệm, chép chính tả, và ghép thẻ.

Dự án được xây dựng nhằm mục đích thực hành và áp dụng các công nghệ .NET mới nhất, cùng với kiến trúc phần mềm hiện đại như Clean Architecture và CQRS.

---

## ✨ Tính năng chính

* ✅ **Quản lý Chủ đề (Topics):** Tạo, xem, sửa, xóa các chủ đề từ vựng (ví dụ: "IELTS Reading", "Từ vựng IT", "Tiếng Anh công sở").
* ✅ **Quản lý Từ vựng (Vocabulary):** Thêm từ mới vào các chủ đề, kèm theo định nghĩa, ví dụ, và phát âm.
* ✅ **Luyện tập Trắc nghiệm (Multiple Choice):** Hệ thống tự động tạo bài kiểm tra trắc nghiệm dựa trên các chủ đề bạn chọn.
* ✅ **Luyện tập Chép chính tả (Dictation):** Nghe và gõ lại từ vựng để luyện kỹ năng nghe và viết.
* ✅ **Luyện tập Ghép thẻ (Matching):** Kéo thả để ghép từ vựng với định nghĩa hoặc hình ảnh tương ứng.
* ✅ **Giao diện hiện đại:** Sử dụng thư viện Ant Design Blazor cho trải nghiệm người dùng mượt mà và đẹp mắt.

---

## 📸 Demo & Hình ảnh

Bạn có thể xem demo trực tiếp tại đây:
**https://youtu.be/zlP1AwH6dpA**

### Video hướng dẫn/Demo tính năng
[![Video Demo FastVocab](https://img.youtube.com/vi/zlP1AwH6dpA/hqdefault.jpg)](https://www.youtube.com/watch?v=zlP1AwH6dpA)

### Một số ảnh chụp màn hình

| Trang chủ | Luyện tập trắc nghiệm | Chép chính tả | Ghép thẻ |
| :---: | :---: | :---: | :---: |
| <img src="https://res.cloudinary.com/dai6bxxxu/image/upload/v1762335966/words_ygo5hn.jpg" />|<img src="https://res.cloudinary.com/dai6bxxxu/image/upload/v1762335965/quiz_thqbqu.jpg" />|<img src="https://res.cloudinary.com/dai6bxxxu/image/upload/v1762335964/dictation_u6dgrv.jpg" />|<img src="https://res.cloudinary.com/dai6bxxxu/image/upload/v1762337273/matching_zkablp.jpg" />|


---

## 🛠️ Công nghệ & Kiến trúc

Dự án này là sự kết hợp của các công nghệ và phương pháp luận tiên tiến trong hệ sinh thái .NET.

### Công nghệ sử dụng

* **Nền tảng:** .NET 9
* **Database:** SQL Server 2022
* **Frameworks:**
    * **Backend:** ASP.NET Core Web API 9
    * **Frontend:** Blazor Server
* **Data Access:** Entity Framework Core 9 (EF Core)
* **UI Component:** Ant Design Blazor
* **In-Memory Messaging:** MediatR

### Kiến trúc

Dự án được xây dựng dựa trên:
1.  **Clean Architecture:** Phân tách rõ ràng các lớp (Domain, Application, Infrastructure, Presentation) để đảm bảo tính độc lập, dễ bảo trì và dễ kiểm thử (testable).
2.  **CQRS (Command Query Responsibility Segregation):** Tách biệt các luồng xử lý "Ghi" (Commands) và "Đọc" (Queries) để tối ưu hóa hiệu suất và đơn giản hóa logic nghiệp vụ. MediatR được sử dụng để triển khai pattern này một cách hiệu quả.

---

## ⚙️ Hướng dẫn cài đặt

Để chạy dự án này local, bạn cần:
* [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
* [SQL Server 2022](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (hoặc SQL Server Express/Developer Edition)
* Một IDE như [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) hoặc [VS Code](https://code.visualstudio.com/).

### Các bước thực hiện

1.  **Clone repository:**
    ```bash
    git clone https://github.com/annghdev/FastVocab.git
    cd FastVocab
    ```

2.  **Cấu hình Database:**
    * Mở file `appsettings.json` hoặc tạo `appsettings.Development.json` trong dự án `FastVocab.Api` (hoặc dự án Infrastructure của bạn).
    * Cập nhật chuỗi `ConnectionStrings` để trỏ đến instance SQL Server 2022 của bạn.
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=YOUR_SERVER;Database=FastVocabDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
    }
    ```

3.  **Áp dụng Migrations:**
    * Mở Terminal/Command Prompt trong thư mục gốc của dự án.
    * Chạy lệnh sau để tạo database và schema (đảm bảo bạn đã cài đặt `dotnet-ef` tool):
    ```bash
    dotnet ef database update --project server/src/FastVocab.Infrastructure
    ```
    * Hoặc dùng tool trong Visual Studio 2022, xem video demo nếu bạn chưa biết

4.  **Chạy ứng dụng:**
    * **Cách 1 (Visual Studio):** Mở file `.sln` và thiết lập "Multiple startup projects". Chọn "Start" cho cả dự án API (`FastVocab.Api`) và dự án Blazor Server (`FastVocab.Blazor`).
    * **Cách 2 (CLI):**
        * Mở 2 terminal.
        * Terminal 1 (chạy API): `dotnet run --project server/src/FastVocab.API`
        * Terminal 2 (chạy Blazor): `dotnet run --project clients/web/FastVocab.BlazorWebApp`

5.  **Truy cập ứng dụng:**
    Mở trình duyệt và truy cập vào địa chỉ mà Blazor Server đang chạy (`https://localhost:7226`).

---

## 📄 Giấy phép

Dự án này được cấp phép theo **Giấy phép MIT**. Xem chi tiết tại file [LICENSE](LICENSE) (Bạn nên tạo một file tên `LICENSE` và dán nội dung giấy phép MIT vào đó).

Bản quyền (c) 2025 annghdev

---

Cảm ơn bạn đã ghé thăm dự án! Nếu bạn thấy hữu ích, đừng quên tặng một ⭐ nhé!
