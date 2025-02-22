import http from 'k6/http';
import {sleep, check} from 'k6';
import {BASE_URL, MASTER_PASSWORD, MASTER_USERNAME} from "../../config/config.js";

export const options = {
    // interaction: 1,
    stages: [
      { duration: '30s', target: 20 },
      { duration: '1m30s', target: 10 },
      { duration: '20s', target: 0 },
    ],
}

export default function() {
    const credentials = {
        username: MASTER_USERNAME,
        password: MASTER_PASSWORD
    };

    const loginHeaders = {
        headers: { 'Content-Type': 'application/json' },
    };

    const resLogin = http.post(`${BASE_URL}/api/Auth/login`, JSON.stringify(credentials), loginHeaders);

    check(resLogin, {
        'Status is 200': (r) => resLogin.status === 200,
    });

    check(resLogin, {
        'Token exists': (r) => resLogin.json().token != null,
    });

    sleep(0.5); // Simula uma pausa de 500ms entre as requisições
}
