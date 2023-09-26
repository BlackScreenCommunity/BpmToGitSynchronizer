# BpmToGitSynchronizer

## Назначение

Выполняет регулярную выгрузку изменений со стенда BPMSoft/Cratio и фиксирует изменения в git-репозитории

## Конфигурирование

Все доступные настройки расположены в файле конфигурации appsettings.json

### Параметр PushPullPeriodInHours

Отвечает за длительность периода синхронизации (в часах). По умолчанию значение установлено в 2 часа -> утилита каждые два часа будет выполнять цикл выгрузки и фиксации изменений.

### Массив PushPullConfiguration

Cодержит параметры сборок и их репозиториев

#### Блок BpmSoft

Данные стенда требуются для формирования запроса на выгрузку изменений в файловую систему.

- BpmSoft.Url - Адрес стенда BPMSoft/Creatio;
- BpmSoft.UserName - Имя пользователя, у которого есть доступ к Конфигурации системы;
- BpmSoft.Password - Пароль пользователя;
- BpmSoft.IsNetCore - Указывает на платформу систему (NetCore или NetFrameWork).

#### Блок GitRepo

Требуется для выполнения команд git.

- GitRepo.Path - путь до локального git-репозитория, каталог Pkg, куда выгружаются изменения BPMSoft;
- GitRepo.UserName - Имя пользователя удаленного git-репозитория;
- GitRepo.Password - Пароль пользователя удаленного git-репозитория;
- GitRepo.Branch - Название текщей ветки git-репозитория;
- GitRepo.CommitMessage - Часть сообщения коммита. Полный коммит формируется по шаблону `{YYYY-MM-DD HH:mm}{CommitMessage}`.

### Пример заполнения файла appsettings.json

``` json
{
    "PushPullPeriodInHours": 2,
    "PushPullConfiguration": [
        {
            "BpmSoft": {
                "Url": "http://localhost",
                "UserName": "Supervisor",
                "Password": "Supervisor",
                "IsNetCore": false
            },
            "GitRepo": {
                "Path": "C:\\Users\\User\\GitRepository",
                "UserName": "email@email.ru",
                "Password": "password",
                "Branch": "master",
                "CommitMessage": "AutoCommit"
            }
        }
    ]
}
```

## Параметры запуска

### Без параметра

Базовый запуск утилиты. Выгрузка-фиксация не запускается, а только планируется на время, указанное в параметре PushPullPeriodInHours.

Запуск в Windows

``` bash
# Запуск в Windows
.\BpmToGitSynchronizer.exe

# Запуск в Linux
dotnet BpmToGitSynchronizer.dll
```

### ForceCommit

Единоразово запускает цикл выгрузки-фиксации изменений.
После завершения одной итерации цикла утилита завершает работу.

Запуск в Windows

``` bash
# Запуск в Windows
.\BpmToGitSynchronizer.exe ForceCommit

# Запуск в Linux
dotnet BpmToGitSynchronizer.dll ForceCommit
```
