
document.getElementById("preventEnterInput").addEventListener("keydown", function (event) {
    if (event.key === "Enter") {
        event.preventDefault();
    }
});

function debounce(func, delay) {
    let timer;
    return function (...args) {
        clearTimeout(timer);
        timer = setTimeout(() => func.apply(this, args), delay);
    };
}

async function saveFieldData(input) {
    const loader = document.createElement('span');
    loader.className = 'loading-indicator';
    input.parentElement.appendChild(loader);
    try {
        console.log("Sending request data:", JSON.stringify({
            categoryId: input.dataset.categoryId,
            itemId: input.dataset.itemId,
            fieldName: input.name,
            value: input.value,
            timestamp: new Date().toISOString()
        }));
        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

        const response = await fetch('/CreateBudget?handler=SaveField', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify({
                categoryId: input.dataset.categoryId ? input.dataset.categoryId.toString() : null,
                itemId: input.dataset.itemId ? input.dataset.itemId.toString() : null,
                fieldName: input.name,
                value: input.value,
                timestamp: new Date().toISOString()
            }),
        });

        console.log(response);

        if (response.ok) {
            loader.className = 'success-indicator';
        } else {
            loader.className = 'error-indicator';
        }
    } catch (error) {
        loader.className = 'error-indicator';
        console.error(error);
    } finally {
        setTimeout(() => loader.remove(), 2000);
    }
}


// Koppla inputfält till debounce-funktionen
document.querySelectorAll('.auto-save-input[data-page="create-budget"]').forEach(input => {
    input.addEventListener('input', debounce(function () {
        saveFieldData(this);
    }, 1000));
});







//function addCategory(type) {
//    const container = document.querySelector(type === 'income' ? '#incomes-container' : '#expenses-container');
//    const index = container.children.length;

//    const categoryHtml = `
//        <div>
//            <input name="Budget.${type === 'income' ? 'Incomes' : 'Expenses'}[${index}].Name" placeholder="Category Name" />
//            <button type="button" onclick="removeCategory('${type}', ${index})">Remove</button>
//            <ul id="${type}-items-${index}">
//            </ul>
//            <button type="button" onclick="addItem('${type}', ${index})">Add Item</button>
//        </div>
//    `;
//    container.insertAdjacentHTML('beforeend', categoryHtml);
//}

//// TODO: Kolla att removeCategory tar bort rätt index
//function removeCategory(type, index) {
//    const container = document.querySelector(type === 'income' ? '#incomes-container' : '#expenses-container');
//    const category = container.children[index];
//    if (category) {
//        container.removeChild(category);
//    }
//}

//function addItem(type, categoryIndex) {
//    const itemList = document.querySelector(`#${type}-items-${categoryIndex}`);
//    if (!itemList) return;

//    const itemIndex = itemList.children.length;
//    const itemHtml = `
//        <li>
//            <input name="Budget.${type === 'income' ? 'Incomes' : 'Expenses'}[${categoryIndex}].Items[${itemIndex}].Name" placeholder="Item Name" />
//            <input name="Budget.${type === 'income' ? 'Incomes' : 'Expenses'}[${categoryIndex}].Items[${itemIndex}].Amount" placeholder="Amount" type="number" />
//            <button type="button" onclick="removeItem('${type}', ${categoryIndex}, ${itemIndex})">Remove</button>
//        </li>
//    `;
//    itemList.insertAdjacentHTML('beforeend', itemHtml);
//}

//function removeItem(type, categoryIndex, itemIndex) {
//    const itemList = document.querySelector(`#${type}-items-${categoryIndex}`);
//    const item = itemList.children[itemIndex];
//    if (item) {
//        itemList.removeChild(item);
//    }
//}