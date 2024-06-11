import React, { useState } from 'react';
import axios from 'axios';
import '../css/auth.css';

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
            const callbackUrl = 'beshare.app://callback?token=' + token;
            window.location.href = callbackUrl;
            console.log(callbackUrl);
            //localStorage.setItem('token', token);
            //console.log(response.data);
            //console.log("Авторизація успішна!");
        } catch (error) {
            setError(error.response.data);
        }
    };

    return (
        <div className="app-container">
            <div className="app-info">
                <div className="app-menu-r">
                    <img src="/contnet/img/img.svg" alt=""/>
                </div>
                <div className="app-menu-s">
                    <h1 className="txt-main">Авторизація</h1>
                    <div className="inp-help">
                        <form onSubmit={handleSubmit}>
                            <div className="inp-info">
                                <label className="txt-normal">Логін</label>
                                <input className="inp-grid" type="text" value={username}
                                       onChange={(e) => setUsername(e.target.value)}/>
                            </div>
                            <div className="inp-info">
                                <label className="txt-normal">Пароль</label>
                                <input className="inp-grid" type="password" value={password}
                                       onChange={(e) => setPassword(e.target.value)}/>
                            </div>
                            <button className="btn" type="submit">Увійти</button>
                            <a href="/register" className="register-link">Створити аккаунт</a>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default LoginForm;