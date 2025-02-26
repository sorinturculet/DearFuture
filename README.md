📌 DearFuture – Time Capsule App
DearFuture is a Time Capsule App that allows users to create digital time capsules containing messages, photos, or videos that remain locked until a specified future date. Whether for personal reflection, surprises, or event planning, DearFuture adds a unique element of nostalgia and anticipation. 🎉
![image](https://github.com/user-attachments/assets/2e947444-2bac-48bf-8885-639abe836749)
## 🌟 Features

- ✅ **Create & Lock Time Capsules** – Store text, images, or videos until a set date.
- ⏳ **Countdown Timer** – See time remaining until capsules unlock.
- 🎨 **Customizable Capsules** – Select colors and categorize capsules.
- 📍 **Location-Based Unlocking** – Unlock capsules only at specific locations.
- 🗄 **Archived Capsules** – Opened capsules move to an archive for future reference.
- 🗑 **Trash Management** – Capsules in trash get auto-deleted after 15 days.
- 📅 **Sorting & Filtering** – Sort by name, date created, or unlock date, and filter by category.
- 🔄 **Observer Pattern** – Capsules dynamically update across the app.
- 🛡 **Secure Storage** – Uses SQLite to store data locally.

![image](https://github.com/user-attachments/assets/e4c08bf5-beec-4a8e-b326-a3fa8fb2eaad)

![image](https://github.com/user-attachments/assets/c8fe6605-1be2-4486-98fa-acf59bf4898a)

## 🚀 Technologies Used

| **Technology**         | **Usage**                                      |
|------------------------|----------------------------------------------|
| **.NET 8 & MAUI**      | Cross-platform support (Windows, Android, iOS) |
| **C#**                | Core application logic                         |
| **SQLite**            | Local database storage                         |
| **Dependency Injection** | Uses `CapsuleService` and `CapsuleRepository` |
| **MVVM Architecture**  | ViewModel-based UI logic                      |
| **Observer Pattern**   | Capsules auto-update across screens           |
| **Geolocation API**    | Used for location-based unlocking             |
