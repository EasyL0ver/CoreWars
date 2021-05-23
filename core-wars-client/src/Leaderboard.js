import React from "react";
import axios from "axios";

import {JsonToTable} from 'react-json-to-table';
import Select from 'react-select'

class Leaderboard extends React.Component {
    constructor(props) {
        super(props)

        this.changeCompetition = this.changeCompetition.bind(this)

        this.state = {
            rows: [],
            categories: [],
            selected: ""
        }
    }

    componentDidMount() {
        this.getCompetitionNames();
    }

    changeCompetition(competition) {
        this.setState({
            selected: competition
        });
        this.getLeaderboard(competition.value);
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
                categories: competitionTypes,
            });

            this.changeCompetition(competitionTypes[0])
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
            <Select options={this.state.categories}  value= {this.state.selected} onChange={this.changeCompetition}/>
            <JsonToTable json={this.state.rows} />
        </div>  
        );
    }

}

export default Leaderboard;