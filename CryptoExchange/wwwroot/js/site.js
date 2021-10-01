// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function applyTemplate(template, item) {
    for (const field of Object.keys(item)) {
        const placeholder = `{{${field}}}`;
        while (template.includes(placeholder)) {
            template = template.replace(placeholder, item[field]);
        }
    }
    return template;
}

function emptyPromise(val = null) {
    return new Promise((resolve) => { resolve(val); });
}