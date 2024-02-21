import React, { useState } from 'react';
import axios from 'axios';

const LoginForm = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const response = await axios.post('https://localhost:7147/account/login', {
                username: username,
                password: password,
            });
            const token = response.data.token;
            localStorage.setItem('token', token);
            console.log(response.data);
            console.log("Авторизація успішна!");

            const callbackUrl = 'beshare.apps://callback?token=' + token;
            window.location.href = callbackUrl;
            console.log(callbackUrl);

        } catch (error) {
            setError(error.response.data);
        }
    };

    return (
        <div>
            <h2>Вхід</h2>
            {error && <p>{error}</p>}
            <form onSubmit={handleSubmit}>
                <div>
                    <label>Ім'я користувача:</label>
                    <input type="text" value={username} onChange={(e) => setUsername(e.target.value)} />
                </div>
                <div>
                    <label>Пароль:</label>
                    <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} />
                </div>
                <button type="submit">Увійти</button>
            </form>
        </div>
    );
};

export default LoginForm;