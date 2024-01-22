// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$('.message a').click(function () {
    $('form').animate({ height: "toggle", opacity: "toggle" }, "slow");
});




/*let animalTypeSelect = document.querySelector('#AnimalType');
let breedSelect = document.querySelector('#Breed');

// Обработчик события изменения выбора типа животного
animalTypeSelect.onchange = function () {
    // Проверка выбранной опции
    if (animalTypeSelect.value === 'cat') {
        // Если выбрана опция 'cat', добавляем опции для пород кошек
        breedSelect.innerHTML = '';
        let catBreeds = ['British Shorthair', 'Siamese', 'Persian'];
        for (let i = 0; i < catBreeds.length; i++) {
            let option = document.createElement('option');
            option.value = catBreeds[i];
            option.text = catBreeds[i];
            breedSelect.appendChild(option);
        }
    } else if (animalTypeSelect.value === 'dog') {
        // Если выбрана опция 'dog', добавляем опции для пород собак
        breedSelect.innerHTML = '';
        let dogBreeds = ['Labrador Retriever', 'German Shepherd', 'Poodle'];
        for (let i = 0; i < dogBreeds.length; i++) {
            let option = document.createElement('option');
            option.value = dogBreeds[i];
            option.text = dogBreeds[i];
            breedSelect.appendChild(option);
        }
    } else {
        // Если выбрана другая опция, очищаем список пород
        breedSelect.innerHTML = '';fruits
    }
}*/
/*
function handleAnimalTypeChange() {
    // получим выбранное значение из первого списка
    var animalTypeSelect = document.getElementById("AnimalType");
    var selectedAnimalType = animalTypeSelect.options[animalTypeSelect.selectedIndex].value;
    console.log(selectedAnimalType);
    var breedSelect = document.getElementById("breedSelect");

    // в зависимости от выбранного типа животного, заполним второй список соответствующими породами
    switch (selectedAnimalType) {
        case "Кот":
            breedSelect.innerHTML = "";
            BreedsCats.forEach(breed => {
                breedSelect.innerHTML += <option>${breed}</option>;
            });
            break;
        case "Собака":
            breedSelect.innerHTML = "";
            BreedsDogs.forEach(breed => {
                breedSelect.innerHTML += <option>${breed}</option>;
            });
            break;
        // другие варианты...
    }
}

animalTypeSelect.addEventListener('change', handleAnimalTypeChange);*/