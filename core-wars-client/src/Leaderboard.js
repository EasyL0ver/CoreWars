import React from "react";
import axios from "axios";

import Select from 'react-select'
import UserStore from './UserStore'
import CoreSelect from './CoreSelect'

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
        this.getLeaderboard(competition);
    }


    async getCompetitionNames() {
        try {
            const response = await axios.get(
                "http://localhost:5000/competitions",
                { "Content-Type": "application/json" }
            );
            const competitionTypes = response.data.map(x => new Object({ value: x.name, label: x.name }));

            this.setState({
                ...this.state,
                categories: competitionTypes,
            });

            this.changeCompetition(competitionTypes[0].value)
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

        const tableRows = this.state.rows.map(row => {
            let rowClass = "rowEntry"

            if (row.creatorId == UserStore.userId)
                rowClass = "userRowEntry"

            return (
                <tr className={rowClass} key={row.scriptId}>
                    <th>{row.alias}</th>
                    <th>{row.creator}</th>
                    <th>{row.language}</th>
                    <th>{row.gamesPlayed}</th>
                    <th>{row.wins}</th>
                    <th>{row.winRate}</th>
                </tr>
            )
        })

        return (
            <div>
                <CoreSelect options={this.state.categories} value={this.state.selected} onChange={this.changeCompetition} />

                <table>
                    <thead>
                        <tr>
                            <th>Alias</th>
                            <th>Creator</th>
                            <th>Language</th>
                            <th>Played</th>
                            <th>Won</th>
                            <th>Win rate</th>
                        </tr>
                    </thead>
                    <tbody>{tableRows}</tbody>
                </table>
            </div>
        );
    }

}

export default Leaderboard;