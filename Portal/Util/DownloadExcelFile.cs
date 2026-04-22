using Microsoft.JSInterop;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Util
{
	public class DownloadExcelFile<T>(IJSRuntime _JS)
	{
		#region Inyeccion Dependencias

		private readonly IJSRuntime JS = _JS;

		#endregion Inyeccion Dependencias

		public async Task<byte[]> DownloadExcel(List<T> Listado, string nameFile)
		{
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			using var package = new ExcelPackage();
			ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(nameFile);

			// Escribir encabezados de columnas
			int columnIndex = 1;

			List<PropertyInfo> listProperties = [.. typeof(T).GetProperties()];

			listProperties.ForEach(x =>
			{
				worksheet.Cells[1, columnIndex].Value = x.Name;
				columnIndex++;
			});

			// Escribir datos de la lista en el archivo Excel
			int rowIndex = 2;
			Listado.ForEach(x =>
			{
				columnIndex = 1;

				listProperties.ForEach((z) =>
				{
					var value = z.GetValue(x);
					worksheet.Cells[rowIndex, columnIndex].Value = value?.ToString() ?? string.Empty;
					columnIndex++;
				});

				rowIndex++;
			});

			// Convertir el paquete a un array de bytes
			byte[] excelBytes = await package.GetAsByteArrayAsync();
			return excelBytes;
		}

		public async Task DownloadFileFromStream(byte[] archivo, string nombrearchivo)
		{
            MemoryStream fileStream = new(archivo);
            using DotNetStreamReference streamRef = new(stream: fileStream);

            await JS.InvokeVoidAsync("downloadFileFromStream", $"{nombrearchivo}.xlsx", streamRef);
        }
	}
}