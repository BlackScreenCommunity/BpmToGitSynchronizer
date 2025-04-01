# BpmToGitSynchronizer

## Назначение

Выполняет регулярную выгрузку изменений со стенда BPMSoft/Cratio и фиксирует изменения в git-репозитории

## Конфигурирование

Все доступные настройки расположены в файле конфигурации appsettings.json

### Параметр PushPullPeriodInMinutes

Отвечает за длительность периода синхронизации (в минутах). По умолчанию значение установлено в 2 часа -> утилита каждые два часа будет выполнять цикл выгрузки и фиксации изменений.

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
    "PushPullPeriodInMinutes": 2,
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

Базовый запуск утилиты. Выполняется выгрузка-фиксация изменений, затем планируется следующий запуск через период, указанный в параметре PushPullPeriodInMinutes.

Запуск в Windows

``` bash
# Запуск в Windows
.\BpmToGitSynchronizer.exe

# Запуск в Linux
dotnet BpmToGitSynchronizer.dll
```

### Commit (ForceCommit)

Единоразово запускает цикл выгрузки-фиксации изменений.
После завершения одной итерации цикла утилита завершает работу.

Запуск в Windows

``` bash
# Запуск в Windows
.\BpmToGitSynchronizer.exe Commit

# Запуск в Linux
dotnet BpmToGitSynchronizer.dll Commit
```
## Запуск утилиты как сервиса на Linux

``` bash
# Создать и отредактировать файл
sudo vim /etc/systemd/system/bpm_to_git_synchronizer.service

# Добавить сервис в автозагрузку и запустить
sudo systemctl enable bpm_to_git_synchronizer.service --now
```

Содержимое файла `bpm_to_git_synchronizer.service`

``` bash
[Unit]
Description=bpm_to_git_synchronizer

[Service]
ExecStart=/usr/bin/dotnet BpmToGitSynchronizer.dll
# Путь до каталога с приложением
WorkingDirectory=/srv/BpmToGitSyncronizer
User=supervisor
Group=supervisor
Restart=always
SyslogIdentifier=bpm_to_git_synchronizer
PrivateTmp=true

[Install]
WantedBy=multi-user.target
```

