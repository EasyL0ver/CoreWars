class ProbePythonScript:
    def probe_variables(self, object):
        return str(vars(object))
    
    def probe_members(self, object):
        return str(dir(object))