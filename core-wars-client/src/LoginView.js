import React from "react";
import axios from "axios";
import { decodeToken } from "react-jwt";

import "./LoginView.css"


class LoginView extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            email: "",
            password: "",
        };

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    loginUser(state) {
        const headers = {
            'Content-Type': 'application/json',
        }

        axios
            .post(
                "http://localhost:5000/Authentication",
                {
                    username: state.email,
                    password: state.password
                },
                { headers: headers }
            )
            .then((response) => {

                const token = response.data.token
                const decodedToken = decodeToken(token)
                const user = {
                    email: decodedToken['email'],
                    id: decodedToken['user-id']
                }
                this.props.loginAction(user, token)
            })
            .catch((error) => {
                console.log(error);
            });
    }

    handleChange(event) {
        this.setState({
            [event.target.name]: event.target.value,
        });
        event.preventDefault();
    }

    handleSubmit(event) {
        this.loginUser(this.state)
        event.preventDefault();
    }

    render() {
        return (
            <div className="login-container core-popup">
                <h1>Sign in</h1>
                <form onSubmit={this.handleSubmit}>
                    <label className="login-form-label">email address</label>
                    <input
                        className="core-form-input"
                        name="email"
                        type="email"
                        value={this.state.email}
                        onChange={this.handleChange}
                    />
                    <label className="login-form-label">password</label>
                    <input
                        className="core-form-input"
                        name="password"
                        type="password"
                        value={this.state.password}
                        onChange={this.handleChange}
                    />
                    <input className="core-submit-button" type="submit" value="Submit" />
                    <p className="login-form-info">
                        Need an account? <a href="/createAccount">Sign up</a>
                    </p>
                </form>
            </div>
        );
    }
}

export default LoginView;