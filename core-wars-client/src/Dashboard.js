import React from "react";
import axios from "axios";

import CompetitorButton from './CompetitorButton'
import UserStore from './UserStore'
import CompetitorEditView from "./CompetitorEditView";

class Dashboard extends React.Component {
    constructor(props) {
        super(props)

        this.state = {
            competitors: [],
            selectedBtnId: null
        }
    }

    componentDidMount() {
        this.loadCompetitors();
    }

    async loadCompetitors() {
        try {
            let token = UserStore.token;
            const response = await axios.get(
                "http://localhost:5000/Competitors/",
                {
                    "Content-Type": "application/json",
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                }
            );

            this.setState({
                ...this.state,
                competitors: response.data,
                selectedBtnId: response.data[0].id
            });
        } catch (err) {
            console.warn(err);
        }
    }

    onButtonEdit(buttonId) {
        console.log(buttonId)
        this.setState({
            ...this.state,
            selectedBtnId: buttonId,
        });
    }

    render() {
        const buttons = this.state.competitors.map((competitor) => {
            let competitorSelected = competitor.id == this.state.selectedBtnId
            return (
                <CompetitorButton
                    alias={competitor.alias}
                    competitionName={competitor.competition}
                    scriptingLanguage={competitor.language}
                    id={competitor.id}
                    gamesPlayed={competitor.gamesPlayed}
                    gamesWon={competitor.gamesWon}
                    status={competitor.status}
                    onClicked={this.onButtonEdit.bind(this)}
                    highlight={competitorSelected}
                />
            )
        });

        if(this.state.selectedBtnId == null)
        {
            return (
                <div>
                    <div> {buttons} </div>
                    <CompetitorEditView />
                </div>
            );
        }

        const editedCompetitor = this.state.competitors
            .find((competitor) => competitor.id === this.state.selectedBtnId)


        return (
            <div>
                <div> {buttons} </div>
                <CompetitorEditView 
                    alias = {editedCompetitor.alias}
                    code = {editedCompetitor.code}/>
            </div>
        );
    }

}

export default Dashboard;