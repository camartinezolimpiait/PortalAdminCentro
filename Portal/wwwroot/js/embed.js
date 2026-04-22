export function embedReport(containerId, reportId, embedUrl, token, namePages, plataforma) {
    var reporteContainer = document.getElementById(containerId);

    var models = window['powerbi-client'].models;

    var config = {
        type: 'report',
        id: reportId,
        embedUrl: embedUrl,
        accessToken: token,
        permissions: models.Permissions.All,
        tokenType: models.TokenType.Embed,
        viewMode: models.ViewMode.View,
        pageName: namePages,
        settings: {
            panes: {
                filters: { expanded: false, visible: false },
                pageNavigation: { visible: false }
            },
            bars: {
                statusBar: {
                    visible: true
                }
            }
        }
    }

    var report = powerbi.embed(reporteContainer, config);

    var heightBuffer = 32;
    var newHeight = $(window).height() - ($("header").height() + heightBuffer);
    $("#" + containerId).height(newHeight);
    $(window).resize(() => {
        var newHeight = $(window).height() - ($("header").height() + heightBuffer);
        $("#" + containerId).height(newHeight);
    })

    function renderPageButtons(pages) {
        const pageButtonsContainer = document.getElementById('pageButtons');
        pageButtonsContainer.innerHTML = '';  // Clear any previous buttons

        pages.forEach(function (page) {
            const button = document.createElement('button');
            button.textContent = page.displayName;  // Use displayName for the button text
            button.onclick = function () {
                // Set the selected page as active when button is clicked
                page.setActive();
            };
            pageButtonsContainer.appendChild(button);
        });
    }

    // Fetch the pages and filter by display name including "CRC"
    function filterPagesAndRenderButtons() {
        report.getPages().then(function (pages) {
            const filteredPages = pages.filter(function (page) {
                return page.displayName.includes(plataforma);
            });

            // Render buttons for the filtered pages
            renderPageButtons(filteredPages);
        });
    }

    // Listen for the report "loaded" event
    report.on('loaded', function () {
        console.log('Report loaded');
        filterPagesAndRenderButtons();  // Run the function after the report has fully loaded
    });
}