import axios from 'axios';
import React, { useState } from 'react';
import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';
import { useNavigate } from 'react-router-dom'

export default function Login() {
    const title = 'Sign In';
    const navigate = useNavigate();

    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");

    async function handleSubmit(event) {
        event.preventDefault();
        const user = { username, password };
        const response = await axios.post(
            'minitwit', user
        );
        // Condition dependant on API return schema
        if ( response.data ) { 
            let path = 'user_timeline';
            localStorage.setItem('user', response.data);
            navigate(path);
        } else {
            let path = 'public_timeline';
            navigate(path);
        }
    }

    function validateForm() {
        return username.length > 0 && password.length > 0;
    }

    return (
        <div className="Login">
            <Form onSubmit={handleSubmit}>
                <Form.Group controlId="username">
                    <Form.Label>Username</Form.Label>
                    <Form.Control
                        autoFocus
                        type="text"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                    />
                </Form.Group>
                <Form.Group controlId="password">
                    <Form.Label>Password</Form.Label>
                    <Form.Control
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                    />
                </Form.Group>
                <Button type="submit" disabled={!validateForm()}>
                    Login
                </Button>
            </Form>
        </div>
    );
}