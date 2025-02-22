import http from 'k6/http';
import {sleep, check} from 'k6';
import {BASE_URL} from "../../config/config.js";

export const options = {
    // interaction: 1,
    stages: [
        { duration: '30s', target: 20 },
        { duration: '1m30s', target: 10 },
        { duration: '20s', target: 0 },
    ],
}

function getRandomNumber(min, max) {
    return Math.round(Math.random() * (max - min) + min);
}

export default function() {
    const scoreResponse = {
        score: getRandomNumber(1, 10),
        customerName: "CustomerName - Gerado pelos testes de carga",
        comment: "Comment - Gerado pelos testes de carga"
    };

    const headers = {
        headers: { 'Content-Type': 'application/json' },
    }

    const response = http.post(`${BASE_URL}/v1/Nps/Responses`, JSON.stringify(scoreResponse), headers);

    console.log("JSON STRINGIFY: ", JSON.stringify(scoreResponse));
    console.log("RESPONSE: ", response);

    check(response, {
        "Status is 200": (r) => r.status === 200
    });

    sleep(1);
}
