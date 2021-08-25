import React from "react";
import axios from "axios";

import Select from 'react-select'
import CoreSelect from './CoreSelect'

import './Leaderboard.css'

import config from './config.json'

class LandingPage extends React.Component {

    render() {

        let defaultWelcomeMessage = "Welcome to CoreWars"
        if(this.props.user)
            defaultWelcomeMessage = "Welcome " + this.props.user.email


        return (
            <div className="logo-wrapper">
                <div>
                    <img className="logo-image" src={process.env.PUBLIC_URL + '/logo.jpg'}></img>
                </div>
                <div style={{textAlign:'center', fontSize:'50px'}}>
                    {defaultWelcomeMessage}
                </div>
            </div>
        )

    }

}


export default LandingPage