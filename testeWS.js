const socket = new WebSocket('ws://localhost:5255/api/Extract/last');

socket.onopen = function() {
    console.log('Conectado ao servidor WebSocket.');
};

socket.onmessage = function(event) {
    console.log('Mensagem recebida: ', event.data);
};

socket.onclose = function() {
    console.log('Conexão WebSocket fechada.');
};

socket.onerror = function(error) {
    console.log('Erro: ', error);
};
