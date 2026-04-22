// Inicio código generado por GitHub Copilot
// Función para mostrar el modal de exportación de datos
function mostrarModalExportacion() {
    var modal = document.getElementById('modalExportarDatos');
    if (modal && typeof modal.showModal === 'function') {
        modal.showModal();
    } else if (modal) {
        // Fallback para modales que no usan <dialog>
        modal.style.display = 'block';
    }
}
// Fin código generado por GitHub Copilot
