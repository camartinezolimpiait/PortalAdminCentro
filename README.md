#PortalAdminCentro

Aplicación web de administración interna destinada a la gestión de los centros (CRC/CEA) dentro de MiLicencia. Es la interfaz que utiliza el personal administrativo para configurar, supervisar y manejar las operaciones relacionadas con los centros en el sistema. En síntesis, este portal permite a los administradores de las sedes controlar aspectos operativos de MiLicencia de forma centralizada.

# Uso de Tailwind y Daisy UI

Este proyecto usa Tailwind v4 y el plugin de Daisy UI. Consulte la referencia aqui:
- [Tailwind v4](https://tailwindcss.com/docs/styling-with-utility-classes)
- [Daisy UI](https://daisyui.com/components/)

En el `package.json` se encuentran los scripts que compilan Tailwind. No olviden installar las dependencias con `npm install` antes de empezar a trabajar. Requiere la versión LTS de Node.js (v22 en el momento que realizamos esta integración).

## Para desarrollar
La mejor experiencia requiere correr dos procesos en dos terminales separadas:
- `dotnet watch run` para el proyecto de Blazor con hot reload
- `npm run dev:css` para ejecutar el proceso de Tailwind en modo `watch` y así recompilar conforme cambien los archivos

Si están usando Visual Studio 2022, el hot-reload ya lo maneja el IDE, así que basta con crear una nueva terminal y ejecutar únicamente el segundo comando.

El `dotnet watch run` también vigila el archivo compilado en `wwwroot/app.css`, así que un cambio en Tailwind desencadena un evento de hot-reload.

Adicionalmente, para tener la mejor experiencia de auto-completado e intellisense, recomiendo los siguientes ajustes según su editor:

## Con Visual Studio 2022

Instalar la extensión [no-oficial de Tailwind](https://marketplace.visualstudio.com/items?itemName=TheronWang.TailwindCSSIntellisense).

## Con VS Code

Instalar la [extensión oficial de Tailwind](https://marketplace.visualstudio.com/items?itemName=bradlc.vscode-tailwindcss) y crear las siguientes configuraciones:

**Asociar los archivos CSS con tailwindcss**
Esto activa el intellisense y deja de marcar errores sobre la sintaxis de Tailwind

```json
"files.associations": {
  "*.css": "tailwindcss"
}
```

**Decir que trate  los archivos razor como html**
Esto es necesario para activar el intellisense en los archivos .razor del proyecto

```json
"tailwindCSS.includeLanguages": {
  "razor": "html"
}
```