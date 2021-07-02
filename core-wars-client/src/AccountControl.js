import React from "react";
import { Link } from "react-router-dom";

class AccountControl extends React.Component {
    logout(){
        this.props.logoutAction()
    }
    getAccountView(){
        if(this.props.user == null){
            return (<Link key="login" to="/login"> ZALOGUJ </Link>)
        }

        return (
            <div>
                <span>Zalogowano jako: {this.props.user.email}</span>
                <button onClick={this.logout.bind(this)}> Wyloguj </button>
            </div>
        )
    }

    render() {
        return (
            <div>
                {this.getAccountView()}
            </div>
        )
    }
}

export default AccountControl