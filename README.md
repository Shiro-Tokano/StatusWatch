# StatusWatch (В процессе разработки...)

**StatusWatch** — это платформа для мониторинга доступности сервисов, управления инцидентами и отображения публичных страниц статуса.
Проект представляет собой минимальный аналог **Statuspage** или **UptimeRobot**, реализованный на **C# ASP.NET Core** с использованием **PostgreSQL** и **Entity Framework Core**.

## Возможности

* **Проверка сервисов** (HTTP, TCP, ICMP) с настраиваемым интервалом.
* **Сбор метрик** времени отклика, кодов ответа и ошибок.
* **Обнаружение инцидентов** при превышении порога ошибок.
* **Автоматическое закрытие инцидентов** при восстановлении.
* **Хронология событий** по каждому инциденту.
* **REST API** для управления проектами, сервисами, проверками и инцидентами.
* **Фоновый воркер** для выполнения проб в реальном времени.

## Архитектура

Проект состоит из 3 частей:

1. **StatusWatch.Web** — веб-приложение и API.
2. **StatusWatch.Worker** — фоновые задачи (планировщик проб).
3. **StatusWatch.Infrastructure** — база данных, миграции, репозитории.

База данных — **PostgreSQL**, ORM — **Entity Framework Core**.

## Запуск

### 1. Применение миграций

Если база пуста или удалена:

```bash
dotnet ef database update -p src/StatusWatch.Infrastructure -s src/StatusWatch.Web -c AppDbContext
```

### 2. Запуск веб-API

```bash
dotnet run --project src/StatusWatch.Web
```

### 3. Запуск воркера

В отдельном окне:

```bash
dotnet run --project src/StatusWatch.Worker
```

## Примеры использования API

Создать проект:

```powershell
Invoke-RestMethod -Method Post http://localhost:5085/api/projects `
  -ContentType 'application/json' -Body '{"name":"Demo"}'
```

Добавить сервис:

```powershell
Invoke-RestMethod -Method Post http://localhost:5085/api/services `
  -ContentType 'application/json' -Body '{"projectId":1,"name":"Example","url":"https://example.org"}'
```

Добавить HTTP-пробу:

```powershell
Invoke-RestMethod -Method Post http://localhost:5085/api/probes `
  -ContentType 'application/json' -Body '{"serviceId":1,"type":1,"target":"https://example.org","intervalSeconds":30}'
```
