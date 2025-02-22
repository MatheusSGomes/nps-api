import http from 'k6/http';
import {sleep, check} from 'k6';
import {BASE_URL} from "../../config/config.js";
import { getRandomNumber } from "../../common/random_number_generate.js";

export const options = {
    // interaction: 1,
    tags: {
        test_name: "Teste_Carga_Post_Responses",
        environment: "TST"
    },
    thresholds: {
        'http_req_duration{status:200}': ['p(95)<500'], // p95 abaixo de 500ms
        http_req_duration: ['p(90)<500'], // 90% das requisições devem ser < 500ms
    },
    stages: [
        { duration: '30s', target: 20 },
        { duration: '1m30s', target: 10 },
        { duration: '20s', target: 0 },
    ],
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

    check(response, {
        "Status is 200": (r) => r.status === 200,
        "Response timing < 500ms": (r) => r.timings.duration < 500
    });

    sleep(0.5); // Simula uma pausa de 500ms entre as requisições
}
