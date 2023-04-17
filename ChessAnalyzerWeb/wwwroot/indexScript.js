// блокировка ввода данных
function SetEnableStateFindControls()
{
    var childNodes = document.getElementById("FindBlockId").getElementsByTagName('*');
    for (var node of childNodes) {
        node.disabled = true;
    }
}

// Добавление пользователя
async function createUser(name, platform, since, until)
{
    const findModel = {
        userName: name,
        platform: platform,
        since: since,
        until: until
    }

    const response = await fetch(`/api/FindPlayerGames`, {
        method: "POST",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify(findModel)
    });

    if (response.ok === true)
    {
        const res = await response.json();

        if (res) {
            // делаю доступным кнопку анализа
            let blockId = document.getElementById("analyzeBlock");
            //blockId.setAttribute("visibility", "visible");
            blockId.removeAttribute("hidden");
        }
        else
            alert("По заданным параметрам игры не найдены!");
    }
    else {
        const error = await response.json();
        console.log(error.message);
    }
}

// сброс данных формы после отправки
function reset()
{
    document.getElementById("userId").value =
        document.getElementById("userName").value = "";
}

async function CancelAnalyze(controller)
{
    controller.abort();
    alert("Операция анализа прервана!");
}

async function RunAnalyze(controller)
{
    const name = document.getElementById("userName").value;
    const precision = document.getElementById("precision").value;
    //платформа
    const selPl = document.getElementById("platformId");
    const platform = selPl.options[selPl.selectedIndex].value;
    const platformName = selPl.options[selPl.selectedIndex].text;

    if (name && precision && platform)
    {

        const infoData = {
            userName: name,
            precision: precision,
            platform: platform
        }

        const hubConnection = new signalR.HubConnectionBuilder()
            .withUrl("/notifications")
            .withAutomaticReconnect() // 0, 2, 10 и 30 секунд паузы между попытками реконнекта и потом отваливается окончательно
            .configureLogging(signalR.LogLevel.Information)
            .build();

        hubConnection.start().then(() => {

            console.log('Connection started!');
            hubConnection.invoke("JoinGroup", name, platform).catch(function (err) {
                return console.error(err.toString());
            });
        })
            .catch(err => console.log('Error while establishing connection :('));

        hubConnection.on("Notification", function (message) {
            document.getElementById("progressId").textContent = message;
        });

        const response = await fetch(`/api/AnalyzeGames`, {
            method: "POST",
            headers: { "Accept": "application/json", "Content-Type": "application/json" },
            body: JSON.stringify(infoData),
            signal: controller.signal
        });

        // если запрос прошел нормально
        if (response.ok === true) {
            alert("Партии проанализированы");
            window.location.replace(`/api/${name}/${platformName}/Mistakes/1`)
        }
    }
    else
        alert("Не все необходимые данные выбраны для начала анализа!");
}

async function LoadPlatforms()
{
    const response = await fetch(`/api/GetAvailablePlatforms`, {
        method: "Get",
        headers: { "Accept": "application/json", "Content-Type": "application/json" }     
    });

    if (response.ok === true)
    {
        const platform = document.getElementById("platformId");

        const dic = await response.json();
        for (var key in dic)
        {
            var option = document.createElement("option");
            option.text = dic[key];
            option.value = key;
            platform.add(option);
        }
    }
    else
    {
        const error = await response.json();
        console.log(error.message);
    }
}

// для сигнала отмены
const controller = new AbortController();

// загрузка выбора платформ
LoadPlatforms();

// отмена анализа партий
document.getElementById("cancelBtn").addEventListener("click", () =>
{ 
    CancelAnalyze(controller);
    document.getElementById("analyzeBtn").disabled = false;
});

// запуск анализа партий
document.getElementById("analyzeBtn").addEventListener("click", () =>
{
    document.getElementById("analyzeBtn").disabled = true;
    RunAnalyze(controller);
});

// сброс значений формы
document.getElementById("resetBtn").addEventListener("click", () => reset());

document.getElementById("findBtn").addEventListener("click", async () =>
{
    // логин
    const name = document.getElementById("userName").value;

    // дата С
    let since = document.getElementById("dateFrom").value;
    since = since ? new Date(since) : null;

    // Дата По
    let until = document.getElementById("dateTo").value;
    until = until ? new Date(until) : null;

    //платформа
    const selPl = document.getElementById("platformId");
    const platform = selPl.options[selPl.selectedIndex].value;

    if (name && platform)
    {
        SetEnableStateFindControls();
        await createUser(name, platform, since, until);
    }
});