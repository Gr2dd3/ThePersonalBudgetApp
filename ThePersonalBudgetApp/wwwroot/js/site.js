function addCategory(type) {
    const container = document.querySelector(type === 'income' ? '#incomes-container' : '#expenses-container');
    const index = container.children.length;

    const categoryHtml = `
        <div>
            <input name="Budget.${type === 'income' ? 'Incomes' : 'Expenses'}[${index}].Name" placeholder="Category Name" />
            <button type="button" onclick="removeCategory('${type}', ${index})">Remove</button>
            <ul id="${type}-items-${index}">
            </ul>
            <button type="button" onclick="addItem('${type}', ${index})">Add Item</button>
        </div>
    `;
    container.insertAdjacentHTML('beforeend', categoryHtml);
}

function removeCategory(type, index) {
    const container = document.querySelector(type === 'income' ? '#incomes-container' : '#expenses-container');
    const category = container.children[index];
    if (category) {
        container.removeChild(category);
    }
}

function addItem(type, categoryIndex) {
    const itemList = document.querySelector(`#${type}-items-${categoryIndex}`);
    if (!itemList) return;

    const itemIndex = itemList.children.length;
    const itemHtml = `
        <li>
            <input name="Budget.${type === 'income' ? 'Incomes' : 'Expenses'}[${categoryIndex}].Items[${itemIndex}].Name" placeholder="Item Name" />
            <input name="Budget.${type === 'income' ? 'Incomes' : 'Expenses'}[${categoryIndex}].Items[${itemIndex}].Amount" placeholder="Amount" type="number" />
            <button type="button" onclick="removeItem('${type}', ${categoryIndex}, ${itemIndex})">Remove</button>
        </li>
    `;
    itemList.insertAdjacentHTML('beforeend', itemHtml);
}

function removeItem(type, categoryIndex, itemIndex) {
    const itemList = document.querySelector(`#${type}-items-${categoryIndex}`);
    const item = itemList.children[itemIndex];
    if (item) {
        itemList.removeChild(item);
    }
}