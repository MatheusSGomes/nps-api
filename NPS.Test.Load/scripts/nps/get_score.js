import http from 'k6/http';
import {sleep, check} from 'k6';
import {BASE_URL} from "../../config/config.js";
import {generateJwtToken} from "../../common/jwt_generate.js";

export const options = {
    // interaction: 1,
    tags: {
        test_name: "Teste_Carga_Get_Score",
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
};

export default function () {
    const scoreHeaders = {
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + generateJwtToken()
        },
    };

    let res = http.get(`${BASE_URL}/v1/Nps/Score`, scoreHeaders);
    const jsonResponse = res.json();

    check(res, {"Status is 200": (r) => r.status === 200});

    check(res, {
        'Body contains score key': (r) => jsonResponse.score != null,
    });

    sleep(0.5); // Simula uma pausa de 500ms entre as requisições
}
