# Media-Roulette

**Media-Roulette** – это интерактивное веб-приложение, позволяющее пользователям случайным образом выбирать медиа-контент. Проект построен на основе **ASP.NET Core (Blazor)**.

[Посетить сайт Media-Roulette](https://media-random.ru)

## Функциональность

- **Регистрация и авторизация пользователей** Поддерживается аутентификация через Yandex, что упрощает процесс входа и регистрации.
- **Хранение истории**: Проект сохраняет историю выбора медиа-контента и предпочтений пользователей в базе данных, что позволяет легко возвращаться к предыдущим выборам.
- **Интерактивный интерфейс** с использованием **AntDesign**
- **Интеграция с внешними API** для получения медиа-контента (например, Shikimori API)
- **Кеширование данных** с использованием **Redis**

## 💻 Технологический стек

- **Frontend**: Blazor (Server + WebAssembly), AntDesign, JavaScript
- **Backend**: ASP.NET Core, Entity Framework Core
- **Базы данных**: PostgreSQL для реляционных данных, Redis для кеширования
- **Почтовый сервис**: SMTP-сервер для отправки писем с подтверждением регистрации
- **API**: Взаимодействие с внешними API, включая Shikimori для получения данных о контенте
- **DevOps**: Docker, GitHub Actions для автоматизации процессов сборки и деплоя
- **Логирование**: Настроены консольное и отладочное логирование для отслеживания работы приложения
- **API-документация**: Использование **Swagger** для автоматической генерации и тестирования API.
