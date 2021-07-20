import React from "react";
import axios from "axios";

import "./LoginView.css"

import config from './config.json'
import { withRouter } from 'react-router-dom'


class CreateAccountView extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            email: "",
            password: "",
            passwordConfirm: ""
        };

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.loginUser = this.loginUser.bind(this);
    }

    loginUser(state) {
        const headers = {
            'Content-Type': 'application/json',
        }

        axios
            .post(
                config.api + "/Users",
                {
                    username: state.email,
                    password: state.password
                },
                { headers: headers }
            )
            .then((response) => {
                this.props.history.push("/")
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
                <h1>Create account</h1>
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
                    <label className="login-form-label">confirm password</label>
                    <input
                        className="core-form-input"
                        name="passwordConfirm"
                        type="password"
                        value={this.state.passwordConfirm}
                        onChange={this.handleChange}
                    />
                    <input className="core-submit-button" type="submit" value="Submit" />
                </form>
            </div>
        );
    }
}

export default withRouter(CreateAccountView);