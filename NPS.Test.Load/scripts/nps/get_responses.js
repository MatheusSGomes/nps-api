import http from 'k6/http';
import {sleep, check} from 'k6';
import {BASE_URL} from "../../config/config.js";
import {generateJwtToken} from "../../common/jwt_generate.js";

export const options = {
    // interaction: 1,
    stages: [
      { duration: '30s', target: 20 },
      { duration: '1m30s', target: 10 },
      { duration: '20s', target: 0 },
    ],
};

export default function () {
    const headers = {
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + generateJwtToken()
        },
    };

    let res = http.get(`${BASE_URL}/v1/Nps/Responses`, headers);

    check(res, {"Status is 200": (r) => r.status === 200});

    check(res, {
        "Body contains multiples results": (r) => r.json().length > 0,
        "Body first result contains 'score' key": (r) => r.json().score !== null,
        "Body first result contains 'customerName' key": (r) => r.json().customerName !== null,
        "Body first result contains 'comment' key": (r) => r.json().comment !== null,
    });

    sleep(0.5); // Simula uma pausa de 500ms entre as requisições
}
