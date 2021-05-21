import React from "react";
import axios from "axios";

import {JsonToTable} from 'react-json-to-table';
import Select from 'react-select'

class Leaderboard extends React.Component {
    constructor(props) {
        super(props)

        this.state = {
            rows: [],
            categories: []
        }
    }

    componentDidMount() {
        this.getCompetitionNames();
    }


    async getCompetitionNames() {
        try {
            const response = await axios.get(
                "http://localhost:5000/competitions",
                null,
                { "Content-Type": "application/json" }
            );
            const competitionTypes = response.data.map(x => new Object({value: x.name, label: x.name}));

            this.setState({
                ...this.state,
                categories: competitionTypes
            });
            this.getLeaderboard(competitionTypes[0].value);
        } catch (err) {
            console.warn(err);
        }
    }

    async getLeaderboard(competitionName) {
        try {
            const response = await axios.get(
                "http://localhost:5000/Leaderboard?competition=" + competitionName,
                null,
                { "Content-Type": "application/json" }
            );
            const leaderBoardRows = response.data;

            this.setState({
                ...this.state,
                rows: leaderBoardRows
            });
        } catch (err) {
            console.warn(err);
        }
    }

    render() {
        return (
        <div> 
            <Select options={this.state.categories} />
            <JsonToTable json={this.state.rows} />
        </div>  
        );
    }

}

export default Leaderboard;