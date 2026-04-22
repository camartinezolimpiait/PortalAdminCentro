/* Esta función de JS hace que se cierren los dropdowns de la navegación al navegar entre módulos.
Es necesario porque Blazor Server sólo cambia una parte de la página y no dispara evento de navegación. */

export function dropdownCloser() {
    const navLinks = document.getElementsByClassName("header-link"); //Esto bota un HTMLCollection
    const menus = document.querySelectorAll("details"); //Este sí bota un NodeList donde sirve forEach

    for (let link of navLinks) {
        link.addEventListener("click", closeAllDropdowns);
    }

    function closeAllDropdowns() {
        menus.forEach((deet) => {
            if (deet.open) deet.open = false;
        });
    }
}

