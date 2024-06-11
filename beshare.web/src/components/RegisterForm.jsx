import React, { useState } from 'react';
import axios from 'axios';
import '../css/auth.css';

const RegisterForm = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [email, setEmail] = useState('');
    const [error, setError] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const response = await axios.post('https://localhost:7147/account/register', {
                username: username,
                password: password,
                email: email
            });
            console.log(response.data);

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
                    <h1 className="txt-main">Реєстрація</h1>
                    <div className="inp-help">
                        <form onSubmit={handleSubmit}>
                            <div className="inp-info">
                                <label className="txt-normal">Ім'я користувача</label>
                                <input className="inp-grid" type="text" value={username} onChange={(e) => setUsername(e.target.value)}/>
                            </div>
                            <div className="inp-info">
                                <label className="txt-normal">Почта</label>
                                <input className="inp-grid" type="email" value={email} onChange={(e) => setEmail(e.target.value)}/>
                            </div>
                            <div className="inp-info">
                                <label className="txt-normal">Пароль</label>
                                <input className="inp-grid" type="password" value={password} onChange={(e) => setPassword(e.target.value)}/>
                            </div>

                            <button className="btn" type="submit">Стоврити</button>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default RegisterForm;