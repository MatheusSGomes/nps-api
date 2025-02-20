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

function generateJwtToken()
{
  const credentials = { username: "admin", password: "password" };
  const loginHeaders = {
    headers: { 'Content-Type': 'application/json' },
  };

  const resLogin = http.post(`${BASE_URL}/api/Auth/login`, JSON.stringify(credentials), loginHeaders);
  return resLogin.json().token;
}

export default function() {
  const scoreHeaders = {
    headers: {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + generateJwtToken()
    },
  };

  let res = http.get(`${BASE_URL}/v1/Nps/Score`, scoreHeaders);
  const jsonResponse = res.json();

  check(res, { "Status is 200": (res) => res.status === 200 });

  check(res, {
    'Body contains score key': (r) => jsonResponse.score != null,
  });

  sleep(1);
}
