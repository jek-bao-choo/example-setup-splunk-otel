const express = require('express');
const axios = require('axios');

const app = express(); // WIP: try to use other web framework such as Nest.js instead of Express so can have

// Middleware to handle JSON requests
app.use(express.json());

// POST endpoint to receive data from the client
app.post('/b', (req, res) => {
    console.log(`Received data from /a: ${JSON.stringify(req.body)}, calling /c`);
    sendData();
    res.status(200).send({ message: 'Get /b successfully' });
});

// WIP: try to send via Kafka instead of through REST API
async function sendData() {
    const data = { message: 'Hello from /b' };
    await axios.post('http://localhost:3003/c', data);
}

// Start the server
app.listen(3002, () => {
    console.log('Server is running on port 3002');
});
