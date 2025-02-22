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
}

export default function() {
    const headers = {
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + generateJwtToken()
        },
    };

    let res = http.get(`${BASE_URL}/v1/Nps/Summary`, headers);

    check(res, {"Status is 200": (res) => res.status === 200});

    check(res, {
        "Body contains 'npsScore' key": (r) => r.json().npsScore != null,
        "Body contains 'promoters' key": (r) => r.json().promoters != null,
        "Body contains 'neutrals' key": (r) => r.json().neutrals != null,
        "Body contains 'detractors' key": (r) => r.json().detractors != null,
    });

    sleep(0.5); // Simula uma pausa de 500ms entre as requisições
}
