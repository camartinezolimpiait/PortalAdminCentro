const tieneSoporteUserMedia = () =>
  !!(
    navigator.getUserMedia ||
    navigator.mozGetUserMedia ||
    navigator.mediaDevices.getUserMedia ||
    navigator.webkitGetUserMedia ||
    navigator.msGetUserMedia
  );
const _getUserMedia = (...arguments) =>
  (
    navigator.getUserMedia ||
    navigator.mozGetUserMedia ||
    navigator.mediaDevices.getUserMedia ||
    navigator.webkitGetUserMedia ||
    navigator.msGetUserMedia
  ).apply(navigator, arguments);

// Declaramos elementos del DOM
var $video = null,
  $stream = null,
  $listaDeDispositivos = null,
  $currCam = null;

const obtenerDispositivos = () => navigator.mediaDevices.enumerateDevices();

function cambiarcamra() {
  $currCam = ($currCam + 1) % $listaDeDispositivos.length;
  mostrarStream($listaDeDispositivos[$currCam].deviceId);
}

function CamaraDiagnostico() {
  if (!tieneSoporteUserMedia()) {
    return 0;
  }

  // Comenzamos pidiendo los dispositivos
  return obtenerDispositivos().then((dispositivos) => {
    // Vamos a filtrarlos y guardar aquí los de vídeo
    const dispositivosDeVideo = [];

    // Recorrer y filtrar
    dispositivos.forEach(function (dispositivo) {
      const tipo = dispositivo.kind;
      if (tipo === "videoinput") {
        dispositivosDeVideo.push(dispositivo);
      }
    });

    // Vemos si encontramos algún dispositivo, y en caso de que si, entonces llamamos a la función
    // y le pasamos el id de dispositivo
    if (dispositivosDeVideo.length > 0) {
      // Mostrar stream con el ID del primer dispositivo, luego el usuario puede cambiar
      return 2;
    } else {
      return 1;
    }
  });
}

function IniciarCaptura() {
  $video = document.querySelector("#video");
  $listaDeDispositivos = document.querySelector("#listaDeDispositivos");
  // Comenzamos viendo si tiene soporte, si no, nos detenemos
  if (!tieneSoporteUserMedia()) {
    alert("Lo siento. Tu navegador no soporta esta característica");
    if (!$estado)
      $estado.innerHTML =
        "Parece que tu navegador no soporta esta característica. Intenta actualizarlo.";
    return false;
  }

  // Comenzamos pidiendo los dispositivos
  obtenerDispositivos().then((dispositivos) => {
    // Vamos a filtrarlos y guardar aquí los de vídeo
    $listaDeDispositivos = [];

    // Recorrer y filtrar
    dispositivos.forEach(function (dispositivo) {
      const tipo = dispositivo.kind;
      if (tipo === "videoinput") {
        $listaDeDispositivos.push(dispositivo);
      }
    });

    // Vemos si encontramos algún dispositivo, y en caso de que si, entonces llamamos a la función
    // y le pasamos el id de dispositivo
    if ($listaDeDispositivos.length > 0) {
      $currCam = 0;
      // Mostrar stream con el ID del primer dispositivo, luego el usuario puede cambiar
      mostrarStream($listaDeDispositivos[0].deviceId);
    } else {
      return false;
    }
  });

  return true;
}

//Escuchar el click del botón para tomar la foto
function TerminarCaptura() {
  ////Pausar reproducción
  $video.pause();

  var myCanvas = document.createElement("canvas");
  //Obtener contexto del canvas y dibujar sobre él
  let contexto = myCanvas.getContext("2d");
  myCanvas.width = $video.videoWidth / 2;
  myCanvas.height = $video.videoHeight / 2;
  contexto.drawImage($video, 0, 0, myCanvas.width, myCanvas.height);

  let foto = myCanvas.toDataURL("image/jpeg", 0.6);

  $stream.getTracks().forEach(function (track) {
    track.stop();
  });

  return foto;
}

function unlockDevice() {
  if ($video && $stream) {
    $video.pause();
    $stream.getTracks().forEach(function (track) {
      track.stop();
    });
  }
}
function mostrarStream(idDeDispositivo) {
  _getUserMedia(
    {
      video: {
        // Justo aquí indicamos cuál dispositivo usar
        deviceId: idDeDispositivo,
      },
    },
    (streamObtenido) => {
      if ($stream != null) {
        $stream.getTracks().forEach(function (track) {
          track.stop();
        });
      }
      // Simple asignación
      $stream = streamObtenido;

      // Mandamos el stream de la cámara al elemento de vídeo
      $video.srcObject = streamObtenido;
      $video.play();
    },
    (error) => {
      //console.log("Permiso denegado o error: ", error);
      if (!$estado)
        $estado.innerHTML =
          "No se puede acceder a la cámara, o no diste permiso.";
    }
  );
}
