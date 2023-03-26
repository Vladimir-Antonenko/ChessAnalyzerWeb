﻿// Добавление пользователя
async function createUser(userName) {

    const response = await fetch("/api/${userName}/FindPlayerGames", {
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

    const response = await fetch("/api/CancelAnalyze", {
        method: "GET",
        headers: { "Accept": "application/json" }
    });
    // если запрос прошел нормально
    if (response.ok === true)
        alert("Операция анализа прервана!");
}

async function RunAnalyze() { // добавить сюда precision !!!!!!!!!

    const response = await fetch("/api/RunAnalyze", {
        method: "GET",
        headers: { "Accept": "application/json" }
    });
    // если запрос прошел нормально
    if (response.ok === true)
        alert("Партии проанализированы");
}

// отмена анализа партий
//document.getElementById("cancelBtn").addEventListener("click", () => CancelAnalyze());

// запуск анализа партий
document.getElementById("analyzeBtn").addEventListener("click", () => RunAnalyze());

// сброс значений формы
document.getElementById("resetBtn").addEventListener("click", () => reset());

// отправка формы
document.getElementById("findBtn").addEventListener("click", async () => {

    const id = document.getElementById("userId").value;
    const name = document.getElementById("userName").value;
    if (id === "")
        await createUser(name);
});