// Inicio código generado por GitHub Copilot
window.downloadHelper = {
 downloadCsv: function (fileName, csvContent) {
 if (!csvContent) {
 return;
 }

 try {
 const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
 const url = URL.createObjectURL(blob);

 const anchor = document.createElement('a');
 anchor.href = url;
 anchor.download = fileName || 'export.csv';

 document.body.appendChild(anchor);
 anchor.click();

 document.body.removeChild(anchor);
 URL.revokeObjectURL(url);
 } catch (e) {
 console.error('Error al descargar el CSV', e);
 }
 },

 openExportSuccessModal: function () {
 try {
 const modal = document.getElementById('modalExportarDatos');
 if (modal && typeof modal.showModal === 'function') {
 modal.showModal();
 }
 } catch (e) {
 console.error('Error al abrir el modal de exportación', e);
 }
 }
};
// Fin código generado por GitHub Copilot
