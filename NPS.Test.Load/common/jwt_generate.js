import http from 'k6/http';
import {BASE_URL, MASTER_PASSWORD, MASTER_USERNAME} from "../config/config.js";

export function generateJwtToken()
{
    const credentials = { username: MASTER_USERNAME, password: MASTER_PASSWORD };
    const loginHeaders = {
        headers: { 'Content-Type': 'application/json' },
    };

    const resLogin = http.post(`${BASE_URL}/api/Auth/login`, JSON.stringify(credentials), loginHeaders);
    return resLogin.json().token;
}
