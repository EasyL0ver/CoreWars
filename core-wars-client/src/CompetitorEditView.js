import React from "react";
import Editor from "react-simple-code-editor";


import { highlight, languages } from "prismjs/components/prism-core";
import "prismjs/components/prism-python";
import "prismjs/themes/prism.css"; //

import CoreSelect from './CoreSelect'

import './LoginView.css'
import './CompetitorEditView.css'



class CompetitorEditView extends React.Component {
    constructor(props) {
        super(props)

        this.state = {
            selected: this.getDefaultCategory(),
            code: props.code,
            alias: props.alias
        }

        this.handleSubmit = this.handleSubmit.bind(this)
        this.onCodeChanged = this.onCodeChanged.bind(this)
        this.onAliasChanged = this.onAliasChanged.bind(this)
    }

    componentDidMount(){
        console.log("COMPONTENT MOUNT!")
        if(!this.state.code && this.state.selected){
            this.fetchTemplate(this.state.selected)
        }
    }

    componentDidUpdate(prevProps, prevState) {
        if (prevProps.categories.join() != this.props.categories.join()) {
            this.changeCompetition(this.getDefaultCategory())
        }
    }

    getDefaultCategory() {
        if(!this.props.categories[0]) return null
        return this.props.categories[0].value
    }

    submitCompetitor() {
        const competitor = {
            alias: this.state.alias,
            code: this.state.code,
            competition: this.state.selected,
            language: "python"
        }

        console.log(competitor)

        console.log("SUBMITTING COMPETITOR!")
        this.props.submit(competitor)
    }

    deleteCompetitor() {
        this.props.delete()
    }

    handleSubmit(event) {
        event.preventDefault()
        const submitType = event.nativeEvent.submitter.name

        if (submitType == "submit") {
            this.submitCompetitor()
            return
        }

        this.deleteCompetitor()
    }

    changeCompetition(competition) {
        this.setState({
            ...this.state,
            selected: competition,
        });

        this.fetchTemplate(competition)
    }

    async fetchTemplate(competitionName) {
        const templatePath = process.env.PUBLIC_URL + "/templates/" + competitionName + ".py"
        const res = await fetch(templatePath);
        const data = await res.text();
        console.log(data);
        this.setState({
            ...this.state,
            code: data,
        });
      }

    onCodeChanged(event){
        console.log(event)
        this.setState({
            ...this.state,
            code: event,
        });
    }

    onAliasChanged(event){
        console.log(event)
        this.setState({
            ...this.state,
            alias: event.target.value,
        });
    }

    render() {
        let deleteButton = (<div></div>)

        if (this.props.delete != null) {
            deleteButton = (<input className="core-submit-button submit-button" type="submit" name='delete' value="Delete"></input>)
        }

        return (
            <div key={this.props.id}>
                <form className="edit-competitor-form" onSubmit={this.handleSubmit} key={this.props.id}>

                    <div className="form-head-container">
                        <div className="name-edit-container">
                            <label className="login-form-label name-input">Agent name:</label>
                            <input className="core-form-input name-input name-text-box" type="text" value={this.state.alias} onChange={this.onAliasChanged} />
                        </div>
                        <div className="type-edit-container">
                            <label className="login-form-label name-input">Competition:</label>
                            <CoreSelect className="type-select type-input" options={this.props.categories} value={this.state.selected} onChange={this.changeCompetition.bind(this)} />
                        </div>
                    </div>
                    <div className="code-text-area-wrapper">
                        <label className="login-form-label name-input">Agent source code:</label>
                        <div className="editor-wrapper">
                            <Editor value={this.state.code} 
                                    onValueChange={this.onCodeChanged}
                                    className="editor"
                                    highlight={(code) => highlight(code, languages.py)}
                                    padding={10}
                                    style={{
                                        fontFamily: '"Courier New", Courier, monospace',
                                        fontSize: '26px',
                                        minHeight: '480px',
                                        border: '0px solid #ccc',
                                        outline: 'none'
                                        // display: 'block',
                                        // overflowY: 'auto'
                                    }}/>
                        </div>
                        {/* <textarea className="code-text-area" defaultValue={this.props.code} ref={this.codeTextArea}></textarea> */}
                    </div>
                    <div className="bottom-area">
                        <div className="button-panel submit-button">
                            <input className="core-submit-button submit-button" name='submit' type="submit" value="Submit" />
                            {deleteButton}
                        </div>
                        <div className="exception-wrapper">
                            <span className="exception-span">{this.props.exception}</span>
                        </div>
                    </div>

                </form>
            </div>)
    }

}


export default CompetitorEditView;