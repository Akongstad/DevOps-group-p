import React, { useState } from 'react';
export default function Register() {
    const title = 'Sign Up';

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");

    function handleSubmit(event) {
        event.preventDefault();
    }

    function validateForm() {
        return email.length > 0 && password.length > 0;
    }

    return (
        <div className="SignUp">
            <Form onSubmit={handleSubmit}>
                <Form.Group controlId="email">
                    <Form.label>Email</Form.label>
                    <Form.Control
                        autoFocus
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                    />
                </Form.Group>
                <Form.Group controlId="password">
                    <Form.Label>Create Password</Form.Label>
                    <Form.Control
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.targe.value)}
                    />
                </Form.Group>
                <Form.Group controlId="password">
                    <Form.Label>Retype Password</Form.Label>
                    <Form.Control
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.targe.value)}
                    />
                </Form.Group>
                <Button type="submit" disabled={!validateForm()}>
                    Register 
                </Button>
            </Form>
        <div>
    );
}
