import http from 'k6/http';
import { sleep, check } from 'k6';
import {BASE_URL} from "../config/config";

export const options = {
  // vus: 10,
  // duration: '30s',
  stages: [
    { duration: '30s', target: 20 },
    { duration: '1m30s', target: 10 },
    { duration: '20s', target: 0 },
  ],
};

export default function() {
  let res = http.get(`${BASE_URL}/v1/Nps/Score`);

  check(res, { "status is 200": (res) => res.status === 200 });

  check(response, {
    'Corpo contém conteúdo esperado': (r) => r.body.includes('score'),
  });

  check(response, {
    'Contém a chave score': (r) => {
      const jsonBody = JSON.parse(r.body);
      return jsonBody.score != null;
    },
  });

  sleep(1);
}
