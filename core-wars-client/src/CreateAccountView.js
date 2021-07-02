import React from "react";
import axios from "axios";
import { decodeToken } from "react-jwt";


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
    }

    loginUser(state) {
        const headers = {
            'Content-Type': 'application/json',
        }

        axios
            .post(
                "http://localhost:5000/Users",
                {
                    username: state.email,
                    password: state.password
                },
                { headers: headers }
            )
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
            <div className="container_login">
                <form className="login_form" onSubmit={this.handleSubmit}>
                    <label className="login_form__label">adres email</label>
                    <input
                        className="login_form__input"
                        name="email"
                        type="email"
                        value={this.state.email}
                        onChange={this.handleChange}
                    />
                    <label className="login_form__label">hasło</label>
                    <input
                        className="login_form__input"
                        name="password"
                        type="password"
                        value={this.state.password}
                        onChange={this.handleChange}
                    />
                    <label className="login_form__label">powtórz hasło</label>
                    <input
                        className="login_form__input"
                        name="passwordConfirm"
                        type="passwordConfirm"
                        value={this.state.passwordConfirm}
                        onChange={this.handleChange}
                    />
                    <input className="login_form__submit" type="submit" value="Zaloguj" />
                </form>
            </div>
        );
    }
}

export default CreateAccountView;