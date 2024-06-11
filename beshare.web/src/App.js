import React, { Suspense, lazy } from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import './App.css';

const RegisterForm = lazy(() => import('./components/RegisterForm'));
const LoginForm = lazy(() => import('./components/LoginForm'));
const FileDetailComponent = lazy(() => import('./components/Download'));
const AdminLoginForm = lazy(() => import('./components/AdminLogin'));

function App() {
    return (
        <Router>
            <Suspense fallback={<div>Loading...</div>}>
                <Routes>
                    <Route path="/register" element={<RegisterForm />} />
                    <Route path="/login" element={<LoginForm />} />
                    <Route path="/admin/auth" element={<AdminLoginForm />} />
                    <Route path="/files/info/:id" element={<FileDetailComponent />} />
                </Routes>
            </Suspense>
        </Router>
    );
}

export default App;
