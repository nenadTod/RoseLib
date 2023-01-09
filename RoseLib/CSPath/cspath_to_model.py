from textx import metamodel_from_file 

tx_grammar_path = r".\CSPath\cspath.tx"

cspath_meta = metamodel_from_file(tx_grammar_path)

model = cspath_meta.model_from_str(sentence) # sentence is passed through the scope
path = model.path
path_elements_count = len(path)

