import http from 'k6/http';
import {sleep, check} from 'k6';
import {BASE_URL} from "../../config/config.js";
import {generateJwtToken} from "../../common/jwt_generate.js";

export const options = {
    // vus: 1,
    // duration: '2s',
    interaction: 1,

    // stages: [
    //   { duration: '30s', target: 20 },
    //   { duration: '1m30s', target: 10 },
    //   { duration: '20s', target: 0 },
    // ],
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
