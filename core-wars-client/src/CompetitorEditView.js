import React from "react";

import CoreSelect from './CoreSelect'



class CompetitorEditView extends React.Component {
    constructor(props) {
        super(props)

        this.state = {
            selected: null
        }

        this.handleSubmit = this.handleSubmit.bind(this)

        this.aliasTextBox = React.createRef()
        this.codeTextArea = React.createRef()
    }

    componentDidUpdate(prevProps, prevState) {
        if(prevProps.categories.join() != this.props.categories.join())
            this.setState({
                ...this.state,
                selected: null,
            });
     }

    handleSubmit(event){
        event.preventDefault()

        let selected = this.state.selected

        if(selected == null && this.props.categories.length == 1)
            selected = this.props.categories[0]

        const competitor = {
            alias : this.aliasTextBox.current.value,
            code : this.codeTextArea.current.value,
            competition : selected.value,
            language : "python"
        }

        console.log(competitor)

        console.log("SUBMITTING COMPETITOR!")
        this.props.submit(competitor)
    }

    changeCompetition(competition){
        this.setState({
            ...this.state,
            selected: competition,
        });
    }

    render(){

        let selected = this.state.selected

        if(selected == null && this.props.categories.length == 1)
            selected = this.props.categories[0]

        let deleteButton = (<div></div>)

        if(this.props.delete != null)
        {
            deleteButton = (<button onClick={this.props.delete}>delete !</button>)
        }

        return ( 
        <div> 
            <form onSubmit={this.handleSubmit} key={this.props.id}>
                <input type="text" defaultValue={this.props.alias} ref={this.aliasTextBox}/>
                <CoreSelect options={this.props.categories} value={selected} onChange={this.changeCompetition.bind(this)}/>
                <textarea defaultValue={this.props.code} ref={this.codeTextArea}></textarea>
                <input type="submit" value="Wyślij" />
            </form>
            {deleteButton}
            <span>{this.props.exception}</span>
        </div>)
    }

}


export default CompetitorEditView;