import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import RegisterForm from './components/RegisterForm';
import LoginForm from './components/LoginForm';
import FileDetailComponent from './components/Download';
import AdminLoginForm from './components/AdminLogin';
import BlacklistPage from './components/AdminBlackList';

ReactDOM.render(
    <React.StrictMode>
        <Router>
            <Routes>
                <Route path="/register" element={<RegisterForm />} />
                <Route path="/login" element={<LoginForm />} />
                <Route path="/admin/auth" element={<AdminLoginForm />} />\
                <Route path="/admin/dashboard" element={<BlacklistPage />} />
                <Route path="/files/info/:id" element={<FileDetailComponent />} />
            </Routes>
        </Router>
    </React.StrictMode>,
    document.getElementById('root') // Додана кома перед цим рядком
);
