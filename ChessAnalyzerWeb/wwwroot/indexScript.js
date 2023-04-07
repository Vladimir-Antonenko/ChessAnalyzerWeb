// Добавление пользователя
async function createUser(userName) {

    const response = await fetch(`/api/${userName}/FindPlayerGames`, {
        method: "POST",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify({
            name: userName
        })
    });
    if (response.ok === true) {
        // делаю доступным кнопку анализа
        let blockId = document.getElementById("analyzeBlock");
        //blockId.setAttribute("visibility", "visible");
        blockId.removeAttribute("hidden");
    }
    else {
        const error = await response.json();
        console.log(error.message);
    }
}

// сброс данных формы после отправки
function reset() {
    document.getElementById("userId").value =
        document.getElementById("userName").value = "";
}

async function CancelAnalyze() {

    const response = await fetch(`/api/CancelAnalyze/userName=${name}`, {
        method: "GET",
        headers: { "Accept": "application/json" }

    });
    // если запрос прошел нормально
    if (response.ok === true)
        alert("Операция анализа прервана!");
}

async function RunAnalyze()
{
    const name = document.getElementById("userName").value;
    const precision = document.getElementById("precision").value;

    const hubConnection = new signalR.HubConnectionBuilder()
        .withUrl("/notifications")
        .build();

    hubConnection.start().then(() => {

        console.log('Connection started!');
        hubConnection.invoke("JoinGroup", name).catch(function (err) {
            return console.error(err.toString());
        });
    })
        .catch(err => console.log('Error while establishing connection :('));

    hubConnection.on("Notification", function (message) {
        document.getElementById("progressId").textContent = message;
    });

    const response = await fetch(`/api/AnalyzeGames/userName=${name}&precision=${precision}`, {
        method: "GET",
        headers: { "Accept": "text/html" }
    });

    // если запрос прошел нормально
    if (response.ok === true) {
        alert("Партии проанализированы");
        window.location.replace(`/api/${name}/Lichess/Mistakes/1`)
    }
}

// отмена анализа партий
document.getElementById("cancelBtn").addEventListener("click", () => CancelAnalyze());

// запуск анализа партий
document.getElementById("analyzeBtn").addEventListener("click", () => RunAnalyze());

// сброс значений формы
document.getElementById("resetBtn").addEventListener("click", () => reset());

document.getElementById("findBtn").addEventListener("click", async () => {

    const id = document.getElementById("userId").value;
    const name = document.getElementById("userName").value;
    if (id === "")
        await createUser(name);
});